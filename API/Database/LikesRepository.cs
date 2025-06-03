using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Database
{
    public class LikesRepository(DataContext context, IMapper mapper) : ILikesRepository
    {
        public void AddLike(UserLike like)
        {
            context.Likes.Add(like);
        }

        public void DeleteLike(UserLike like)
        {
            context.Likes.Remove(like);
        }

        public async Task<IEnumerable<int>> GetCurrentUserLikeIds(int currentUserId)
        {
            return await context.Likes
                .Where(x => x.SourceUserId == currentUserId)
                .Select(x => x.TargetUserId)
                .ToListAsync();
        }

        public async Task<UserLike?> GetUserLike(int sourceId, int targetId)
        {
            return await context.Likes.FindAsync(sourceId, targetId);
                
        }

        public async Task<PagedList<MemberDto>> GetUserLikes(LikesParam likesParam)
        {
            var likes = context.Likes.AsQueryable();
            IQueryable<MemberDto> query;

            switch (likesParam.Predicate)
            {
                case "liked":
                    query = likes
                        .Where(x => x.SourceUserId == likesParam.UserId)
                        .Select(t => t.TargetUser)
                        .ProjectTo<MemberDto>(mapper.ConfigurationProvider);
                    break;
                case "likedBy":
                    query = likes
                        .Where(x => x.TargetUserId == likesParam.UserId)
                        .Select(t => t.SourceUser)
                        .ProjectTo<MemberDto>(mapper.ConfigurationProvider);
                    break;
                default:
                    //Get Both Like Each Other only
                    var likeIds = await GetCurrentUserLikeIds(likesParam.UserId);  //people who user liked

                    query = likes
                        .Where(x => likeIds.Contains(x.SourceUserId) && x.TargetUserId == likesParam.UserId)
                        .Select(t => t.SourceUser)
                        .ProjectTo<MemberDto>(mapper.ConfigurationProvider);
                    break;
            }
            return await PagedList<MemberDto>.CreateAsync(query, likesParam.PageNumber, likesParam.PageSize);
        }
    }
}
