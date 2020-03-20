using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.webapi.Data;
using DatingApp.webapi.Dto;
using DatingApp.webapi.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _configuration;
        private readonly IDatingRepository _repoUser;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IMapper _mapper;

        public AuthController(
            IAuthRepository repo, 
            IConfiguration configuration, 
            IDatingRepository repoUser,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IMapper mapper)
        {
            this._repoUser = repoUser;
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._mapper = mapper;
            this._repo = repo;
            this._configuration = configuration;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegistrationDto userForRegistrationDto)
        {
            // userForRegistrationDto.Username = userForRegistrationDto.Username.ToLower();

            // if (await _repo.UserExists(userForRegistrationDto.Username))
            //     return BadRequest("username already exists");

            // var userForCreate = _mapper.Map<User>(userForRegistrationDto);

            // var createdUser = await _repo.Register(userForCreate, userForRegistrationDto.Password);

            // var userListDto = _mapper.Map<UserForListDto>(createdUser);

            // //return CreatedAtRoute()
            // //return Ok(createdUser); cant send the user as its properties like hash/salt which client does not need to know .. send dto instead
            // return CreatedAtRoute("GetUser", new {Controller="Users", Id = createdUser.Id}, userListDto);

            //above is without the identity and below is with identity ..

            var userForCreate = _mapper.Map<User>(userForRegistrationDto);

            var result = await _userManager.CreateAsync(userForCreate, "p@ssword");

            var userListDto = _mapper.Map<UserForListDto>(userForCreate);

            if(result.Succeeded)
            {
                return CreatedAtRoute(
                            "GetUser", 
                            new {Controller="Users", Id = userListDto.Id}, userListDto);
            }

            return BadRequest(result.Errors);           
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            //below exception was for testing the global exception handler  ..
            // try
            // {
            //throw new Exception("You can not login  ..");                
            //}
            // catch(Exception ex)
            // {
            //     return StatusCode(500, "not allowed to login");            
            // }

            //when not using asp.net identity
            // var userFromRepo = await _repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);
            // if (userFromRepo == null) return Unauthorized("Incorrect username or password");

            //with asp.net identity
            var user = await _userManager.FindByNameAsync(userForLoginDto.Username);

            if(user == null) return Unauthorized("user not found in system");

            var result = await _signInManager.CheckPasswordSignInAsync(user, userForLoginDto.Password, false);

            if(result.Succeeded)
            {
                 return Ok(new
                    {
                        token = await GenerateJwtToken(user),
                        user = _mapper.Map<UserForListDto>(user)
                    });
            }

            return Unauthorized("Incorrect username or password");           
        }

        private async Task<string> GenerateJwtToken(User user)
        {
            //return jwt token here  ..
            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier , user.Id.ToString()),
                new Claim(ClaimTypes.Name , user.UserName)
            };

            var roles = await _userManager.GetRolesAsync(user);

            foreach(var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = System.DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}