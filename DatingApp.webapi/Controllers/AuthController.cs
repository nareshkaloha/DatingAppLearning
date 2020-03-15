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
        private readonly IMapper _mapper;

        public AuthController(
            IAuthRepository repo, 
            IConfiguration configuration, 
            IDatingRepository repoUser,
            IMapper mapper)
        {
            this._repoUser = repoUser;
            this._mapper = mapper;
            this._repo = repo;
            this._configuration = configuration;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegistrationDto userForRegistrationDto)
        {
            userForRegistrationDto.Username = userForRegistrationDto.Username.ToLower();

            if (await _repo.UserExists(userForRegistrationDto.Username))
                return BadRequest("username already exists");

            var userForCreate = _mapper.Map<User>(userForRegistrationDto);

            var createdUser = await _repo.Register(userForCreate, userForRegistrationDto.Password);

            var userListDto = _mapper.Map<UserForListDto>(createdUser);

            //return CreatedAtRoute()
            //return Ok(createdUser); cant send the user as its properties like hash/salt which client does not need to know .. send dto instead
            return CreatedAtRoute("GetUser", new {Controller="Users", Id = createdUser.Id}, userListDto);
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


            var userFromRepo = await _repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);

            if (userFromRepo == null) return Unauthorized("Incorrect username or password");

            //return jwt token here  ..
            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier , userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name , userFromRepo.UserName)
            };

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

            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
                user = _mapper.Map<UserForListDto>( await _repoUser.GetUser(userFromRepo.Id))
            });
        }
    }
}