using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.webapi.Data;
using DatingApp.webapi.Dto;
using DatingApp.webapi.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.webapi.Controllers
{
    [Authorize]
    [Route("/api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;

        public UsersController(IDatingRepository repo, IMapper mapper)
        {
            this._mapper = mapper;
            this._repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetUSers()
        {
            var users = await _repo.GetUsers();
            var userDto = _mapper.Map<IEnumerable<UserForListDto>>(users);

            return Ok(userDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUSer(int id)
        {
            var user = await _repo.GetUser(id);
            var userDto = _mapper.Map<UserForDetailedDto>(user);

            return Ok(userDto);
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForUpdate)
        {
            if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var userFromRepo = await _repo.GetUser(id);
            _mapper.Map<UserForUpdateDto, User>(userForUpdate, userFromRepo);

            await _repo.SaveAll();

            return NoContent();
        }
    }
}