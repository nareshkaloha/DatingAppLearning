using System.Linq;
using System.Threading.Tasks;
using DatingApp.webapi.Data;
using DatingApp.webapi.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.webapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;

        public AdminController(DataContext context, UserManager<User> userManager) // use repo pattern .. this is just to save time ..
        {
            this._userManager = userManager;
            this._context = context;
        }

        [HttpGet("userWithRoles")]
        [Authorize("AdminOnly")]
        public async Task<IActionResult> GetUserRoles()
        {
            var userList = await _context.Users
                .OrderBy(ob => ob.UserName)
                .Select(t => new
                {
                    t.Id,
                    t.UserName,
                    Roles = t.UserRoles.Select(s => s.Role.Name).ToList()
                }).ToListAsync();


            return Ok(userList);
        }

        [HttpPost("editRoles/{userName}")]
        [Authorize("AdminOnly")]
        public async Task<IActionResult> UpdateUserRoles(string userName, UserRolesForEditDto userRolesForEditDto)
        {
            var user = await _userManager.FindByNameAsync(userName);

            var existingUserRoles = await _userManager.GetRolesAsync(user);

            var selectedUserRoles  = userRolesForEditDto.Userroles;
            selectedUserRoles = selectedUserRoles ?? new string[] {};

            var addRolesResult = await _userManager.AddToRolesAsync(user, selectedUserRoles.Except(existingUserRoles));

            if(!addRolesResult.Succeeded)
                return BadRequest("Failed to add roles ..");

            var removeRolesResult = await _userManager.RemoveFromRolesAsync(user, existingUserRoles.Except(selectedUserRoles));

            if(!removeRolesResult.Succeeded)
                return BadRequest("Failed to remove roles .. ");

            return Ok(await _userManager.GetRolesAsync(user));
        }

        [HttpGet("photosForModeration")]
        [Authorize("AdminOrModeratorPhotoOnly")]
        public IActionResult PhotosForModeration()
        {
            return Ok("only admin or moderators can access this");
        }
    }
}