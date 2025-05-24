using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Database
{
    public class UserRepository(DataContext dbContext, IMapper mapper) : IUserRepository
    {
        public async Task<MemberDto?> GetMemberAsync(string username)
        {
            return await dbContext.Users
                .Where(x => x.UserName == username)
                .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<MemberDto>> GetMembersAsync()
        {
            return await dbContext.Users
                .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<AppUser?> GetUserByIdAsync(int id)
        {
            return await dbContext.Users
                .Include(x => x.Photos)
                .SingleOrDefaultAsync(x => x.Id == id); 
        }

        public async Task<AppUser?> GetUserByUsernameAsync(string username)
        {
            return await dbContext.Users
                .Include(x => x.Photos)
                .SingleOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await dbContext.Users
                .Include(x => x.Photos)
                .ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await dbContext.SaveChangesAsync() > 0;
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
