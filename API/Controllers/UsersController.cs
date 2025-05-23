using API.Database;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    //[Authorize]  -    this will authorize the incoming request
    //[AllowAnonymous]  -   this will allow all anonymus request without checking
    public class UsersController(DataContext dbContext) : BaseApiController
    {

        [AllowAnonymous]    //this will allow all anonymus request even if we have Authorize at the top of class
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            var users = await dbContext.Users.ToListAsync();

            return users;
        }
        [Authorize]     //This won't work if we have AllowAnonymous attribute at top of class
        [HttpGet("{id:int}")]   //  /api/users/3
        public async Task<ActionResult<AppUser>> GetUser(int id)
        {
            var user = await dbContext.Users.FindAsync(id);
            if (user == null)
                return NotFound();
            return user;
        }

    }
}
    