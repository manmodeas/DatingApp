using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser user);
        Task<IEnumerable<AppUser>> GetUsersAsync();
        Task<AppUser?> GetUserByIdAsync(int id);    //AppUser? -> tells that return null can be one of the return 
        Task<AppUser?> GetUserByUsernameAsync(string username, bool isUnpprovedPhoto = false);
        Task<PagedList<MemberDto>> GetMembersWithFilterAsync(UserParams param);
        Task<PagedList<MemberDto>> GetMembersAsync(PaginationParam param, bool isUnpprovedPhoto);  
        Task<MemberDto?> GetMemberAsync(string username, bool isUnpprovedPhoto);
    }
}
