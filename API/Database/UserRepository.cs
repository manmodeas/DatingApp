using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Database
{
    public class UserRepository(DataContext dbContext) : IUserRepository
    {
        public async Task<AppUser?> GetUserByIdAsync(int id)
        {
            return await dbContext.Users.FindAsync(id);
        }

        public async Task<AppUser?> GetUserByUsernameAsync(string username)
        {
            return await dbContext.Users.SingleOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await dbContext.Users.ToListAsync();
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
