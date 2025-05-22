using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Database
{
    public class DataContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<AppUser> Users { get; set; }
    }
}
