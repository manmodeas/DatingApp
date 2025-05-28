using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface ILikesRepository
    {
        Task<UserLike?> GetUserLike(int sourceId, int targetId);
        Task<PagedList<MemberDto>> GetUserLikes(LikesParam likesParam);
        Task<IEnumerable<int>> GetCurrentUserLikeIds(int currentUserId);
        void DeleteLike (UserLike like);
        void AddLike(UserLike like);
        Task<bool> SaveChangesAsync();
    }
}
