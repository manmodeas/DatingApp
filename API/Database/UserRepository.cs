using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Database
{
    public class UserRepository(DataContext dbContext, IMapper mapper) : IUserRepository
    {
        public async Task<MemberDto?> GetMemberAsync(string username, bool isUnapprovedPhoto)
        {
            var query = dbContext.Users
                .Where(x => x.UserName == username)
                .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
                .AsQueryable();

            if (isUnapprovedPhoto) query = query.IgnoreQueryFilters();
            return await query.SingleOrDefaultAsync();
        }

        public async Task<PagedList<MemberDto>> GetMembersWithFilterAsync(UserParams param)
        {
            var query = dbContext.Users.AsQueryable();
            query = query.Where(x => x.UserName != param.CurerntUserName);

            if(!string.IsNullOrEmpty(param.Gender))
            {
                query = query.Where(x => x.Gender == param.Gender);
            }
            var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-param.MaxAge - 1));
            var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-param.MinAge));

            query = query.Where(x => x.DateOfBirth >= minDob && x.DateOfBirth <= maxDob);

            query = param.OrderBy switch
            {
                "created" => query.OrderByDescending(x => x.Created),
                _ => query.OrderByDescending(x => x.LastActive)
            };

            return await PagedList<MemberDto>.CreateAsync(query.ProjectTo<MemberDto>(mapper.ConfigurationProvider), param.PageNumber, param.PageSize);
        }

        public async Task<PagedList<MemberDto>> GetMembersAsync(PaginationParam param, bool isUnapprovedPhoto)
        {
            var query = dbContext.Users.AsQueryable();

            query = query.OrderByDescending(x => x.LastActive);

            if (isUnapprovedPhoto) query = query.IgnoreQueryFilters();

            return await PagedList<MemberDto>.CreateAsync(query.ProjectTo<MemberDto>(mapper.ConfigurationProvider), param.PageNumber, param.PageSize);
        }

        public async Task<AppUser?> GetUserByIdAsync(int id)
        {
            return await dbContext.Users
                .SingleOrDefaultAsync(x => x.Id == id); 
        }

        public async Task<AppUser?> GetUserByUsernameAsync(string username, bool isUnapprovedPhoto = false)
        {
            var query = dbContext.Users
                .Include(x => x.Photos)
                .AsQueryable();
            if (isUnapprovedPhoto) query = query.IgnoreQueryFilters();
            return await query.SingleOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await dbContext.Users
                .Include(x => x.Photos)
                .ToListAsync();
        }

        public void Update(AppUser user)
        {
            //Its Uneccesary 
            //When we do modify entity in our code and its a entity that we retrive from the database.. then entityframwork will be traking this entity
            //And if we do make change to that entity.. then entityframwork knows about it.
            //Wee added to know there might come a occasion where we need to explicitly mention it
            dbContext.Entry(user).State = EntityState.Modified;
        }
    }
}
