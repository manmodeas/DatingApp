using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

    public class LikesController(ILikesRepository likesRepository) : BaseApiController
    {
        [HttpPost("{targetUserId:int}")]
        public async Task<ActionResult> ToggleLike(int targetUserId)
        {
            var sourceUserId = User.GetUserId();

            if (sourceUserId == targetUserId) return BadRequest("You can not like yourself");

            var existingLike = await likesRepository.GetUserLike(sourceUserId, targetUserId);

            if(existingLike is null)
            {
                var like = new UserLike
                {
                    SourceUserId = sourceUserId,
                    TargetUserId = targetUserId,
                };

                likesRepository.AddLike(like);
            }
            else
            {
                likesRepository.DeleteLike(existingLike);
            }

            if (await likesRepository.SaveChangesAsync()) return Ok();

            return BadRequest("Failed to update like");
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<int>>> GetCurrentUserLikeIds()
        {
            return Ok(await likesRepository.GetCurrentUserLikeIds(User.GetUserId()));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUserLikes([FromQuery] LikesParam likeParam)
        {
            likeParam.UserId = User.GetUserId();

            var users = await likesRepository.GetUserLikes(likeParam);

            return Ok(users);
        }
        
    }
}
