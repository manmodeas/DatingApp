using API.Database;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API.Controllers
{
    [Authorize]         // -    this will authorize the incoming request
    //[AllowAnonymous]  -   this will allow all anonymus request without checking
    public class UsersController(IUserRepository userRepository, IMapper mapper) : BaseApiController
    {

        //[AllowAnonymous]    //this will allow all anonymus request even if we have Authorize at the top of class
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            var users = await userRepository.GetMembersAsync();

            //var userToReturn = mapper.Map<IEnumerable<MemberDto>>(users);
             
            return Ok(users);
        }
        ////[Authorize]     //This won't work if we have AllowAnonymous attribute at top of class
        //[HttpGet("{id:int}")]   //  /api/users/3
        //public async Task<ActionResult<MemberDto>> GetUserById(int id)
        //{
        //    var user = await userRepository.GetUserByIdAsync(id);
        //    if (user == null)
        //        return NotFound();
        //    var userToReturn = mapper.Map<MemberDto>(user);
        //    return userToReturn;
        //}

        [HttpGet("{username}")]   //  /api/users/username
        public async Task<ActionResult<MemberDto>> GetUserByUsername(string username)
        {
            var user = await userRepository.GetMemberAsync(username);
            if (user == null)
                return NotFound();
            //var userToReturn = mapper.Map<MemberDto>(user);
            return user;
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            //one of the way to retrive username
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if(username == null) return BadRequest("No Username found in token");

            var user = await userRepository.GetUserByUsernameAsync(username);

            if (user == null) return BadRequest("Could not find user");

            mapper.Map(memberUpdateDto, user);      //it will change the user and since entityframwork monitoring it 
                                                    //if we call savechanges .. our user will be updated in database



            //there is a corner condition where if we get the same data that is already present in user
            //that means there are no changes in values
            //In that case SaveAll will return false
            //There is a way to handle that by calling -> userRepository.Update(user);
            //before saving,, what it does is it explicitly tells entityframwork to update the values even if they are same
            //but the correct way it to handle it on client side so that we don't get such kind of requests
            //Since it is pointless to save save values again...

            if (await userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to update the user");
        }
    }
}
    