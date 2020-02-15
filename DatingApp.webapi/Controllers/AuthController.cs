using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.webapi.Data;
using DatingApp.webapi.Dto;
using DatingApp.webapi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthRepository repo, IConfiguration configuration)
        {
            this._repo = repo;
            this._configuration = configuration;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegistrationDto userForRegistrationDto)
        {
            userForRegistrationDto.Username = userForRegistrationDto.Username.ToLower();

            if(await _repo.UserExists(userForRegistrationDto.Username))
                return BadRequest("username already exists");

            var createdUser= await _repo.Register(new Model.User() {Username = userForRegistrationDto.Username}, userForRegistrationDto.Password);

            //return CreatedAtRoute()
            //return Ok(createdUser); cant send the user as its properties like hash/salt which client does not need to know .. send dto instead
            return StatusCode(201);
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

            if(userFromRepo == null) return Unauthorized("Incorrect username or password");

            //return jwt token here  ..
            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier , userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name , userFromRepo.Username)
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

           return Ok( new {
               token = tokenHandler.WriteToken(token)
           });
        }
    }
}