using API.Database;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
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
    public class UsersController(IUnitOfWork unitOfWork, IMapper mapper, IPhotoService photoService) : BaseApiController
    {
        //[Authorize(Roles = "Admin")]      //If we keep this then user with role = "Admin" which you can see in token created when loged in will be accessable to this GetUsers
        //[AllowAnonymous]    //this will allow all anonymus request even if we have Authorize at the top of class
        [HttpGet]
        public async Task<ActionResult<PagedList<MemberDto>>> GetUsers([FromQuery] UserParams param)
        {
            param.CurerntUserName = User.GetUserName();
            var users = await unitOfWork.UserRepository.GetMembersWithFilterAsync(param);

            Response.AddPaginationHeader(users);
            //var userToReturn = mapper.Map<IEnumerable<MemberDto>>(users);
             
            return Ok(users);
        }

        [HttpGet("all-users")]
        public async Task<ActionResult<PagedList<MemberDto>>> GetUsers([FromQuery] PaginationParam param)
        {
            var users = await unitOfWork.UserRepository.GetMembersAsync(param, true);   //For now made it so to retrive unpproved photo when called

            Response.AddPaginationHeader(users);
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
        public async Task<ActionResult<MemberDto>> GetUserByUsername(string username, bool isUnapprovedPhoto)
        {
            var user = await unitOfWork.UserRepository.GetMemberAsync(username, isUnapprovedPhoto);
            if (user == null)
                return NotFound();
            //var userToReturn = mapper.Map<MemberDto>(user);
            return user;
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            //one of the way to retrive usernam
            var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUserName());

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

            if (await unitOfWork.Complete()) return NoContent();

            return BadRequest("Failed to update the user");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUserName());

            if (user == null) return BadRequest("cannot upadate user.");

            var result = await  photoService.AddPhotoAsync(file);

            if(result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId,
            };

            user.Photos.Add(photo);

            //For Http Post request, which is used to insert or add new element 
            //as response for successfully create/add new resource 
            //we should be sending location of that resource as a response 
            //rhan just sending sending status code 200 saying ok
            //CreateAtAction = what this does is ,, it add location key in our http header response 
            //stating the location where we can find the photo\
            // in our case it will be 'https://localhost:7286/api/Users/lisa'
            if (await unitOfWork.Complete())
                return CreatedAtAction(nameof(GetUserByUsername), new { username = user.UserName },
                    mapper.Map<PhotoDto>(photo));

            return BadRequest("Problem adding photo");
        }

        [HttpPut("set-main-photo/{photoId:int}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUserName());

            if (user is null) return BadRequest("Could not find user");

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo is null || photo.IsMain || !photo.IsApproved) return BadRequest("Can not use this as main photo");

            var curMainPhoto = user.Photos.FirstOrDefault(x => x.IsMain);

            if (curMainPhoto is not null) curMainPhoto.IsMain = false;

            photo.IsMain = true;

            if (await unitOfWork.Complete())
                return NoContent();

            return BadRequest("Problem setting main photo");
        }

        [HttpPut("approve-photo/{username}")]
        public async Task<ActionResult> ApprovePhoto(string username, int photoid)
        {
            var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(username, true);

            if (user is null) return BadRequest("Could not find user");

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoid);

            if (photo is null || photo.IsApproved) return BadRequest("Can not approve this photo");

            photo.IsApproved = true;

            if (!user.Photos.Any(x => x.IsMain))
            {
                photo.IsMain = true;
            }

            if (await unitOfWork.Complete())
                return NoContent();

            return BadRequest("Problem approving the photo");
        }

        [HttpDelete("delete-photo/{photoId:int}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUserName());

            if (user is null) return BadRequest("Could not find user");

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo is null || photo.IsMain) return BadRequest("This photo can not be deleted");

            if(photo.PublicId is not null)
            {
                var result = await photoService.DeletePhotoAsync(photo.PublicId);

                if (result.Error is not null) return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);

            if (await unitOfWork.Complete()) return Ok();

            return BadRequest("Problem deleting the photo");
        }
    }
}
    