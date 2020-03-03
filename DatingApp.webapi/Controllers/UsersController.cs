using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.webapi.Data;
using DatingApp.webapi.Dto;
using DatingApp.webapi.Helpers;
using DatingApp.webapi.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.webapi.Controllers
{
    [ServiceFilter(typeof(LogUserLastActivity))]
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
        public async Task<IActionResult> GetUSers([FromQuery]UserParamsDto userParams)
        {
            var callerUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var callerUser = await _repo.GetUser(callerUserId);
            userParams.UserId = callerUser.Id;

            if(string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = callerUser.Gender == "male"? "female" : "male";
            }
            
            var users = await _repo.GetUsers(userParams);
            var userDto = _mapper.Map<IEnumerable<UserForListDto>>(users);

            Response.AddPaginationHeader( 
                new PaginationHeaders(
                    userParams.PageNumber, 
                    userParams.PageSize, 
                    users.TotalCount, 
                    users.TotalPages));

            return Ok(userDto);
        }

        [HttpGet("{id}", Name="GetUser")]
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

        [HttpPost]
        [Route("{id}/like/{likeeId}")]
        public async Task<IActionResult> LikeUser(int id, int likeeId)
        {
            if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var likeFromRepo = await _repo.GetLike(id, likeeId);

            if(likeFromRepo !=null)
            {
                return BadRequest("You already liked this user");
            }

            var likeeFromRepo = _repo.GetUser(likeeId);

            if(likeeFromRepo == null)
            {
                return NotFound("likee not found");
            }

            var likeToBeCreated = new Like() { LikerId = id, LikeeId = likeeId};
            _repo.Add(likeToBeCreated);

            if(await _repo.SaveAll())
            {
                return Ok();
            }

            return BadRequest("Failed to like user");
        }
    }
}