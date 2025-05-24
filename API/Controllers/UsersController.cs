using API.Database;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]         // -    this will authorize the incoming request
    //[AllowAnonymous]  -   this will allow all anonymus request without checking
    public class UsersController(IUserRepository userRepository) : BaseApiController
    {

        //[AllowAnonymous]    //this will allow all anonymus request even if we have Authorize at the top of class
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            var users = await userRepository.GetUsersAsync();
             
            return Ok(users);
        }
        //[Authorize]     //This won't work if we have AllowAnonymous attribute at top of class
        [HttpGet("{id:int}")]   //  /api/users/3
        public async Task<ActionResult<AppUser>> GetUserById(int id)
        {
            var user = await userRepository.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();
            return user;
        }

        [HttpGet("{username}")]   //  /api/users/username
        public async Task<ActionResult<AppUser>> GetUserByUsername(string username)
        {
            var user = await userRepository.GetUserByUsernameAsync(username);
            if (user == null)
                return NotFound();
            return user;
        }

    }
}
    