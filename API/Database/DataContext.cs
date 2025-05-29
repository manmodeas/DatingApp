using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Database
{
    public class DataContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<AppUser> Users { get; set; }
        public DbSet<UserLike> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserLike>()
                .HasKey(k => new { k.SourceUserId, k.TargetUserId });

            builder.Entity<UserLike>()
                .HasOne(s => s.SourceUser)
                .WithMany(t => t.LikedByUser)
                .HasForeignKey(t => t.SourceUserId)
                .OnDelete(DeleteBehavior.Cascade);  //Might through error if you are using sql server //change it to NoAction

            builder.Entity<UserLike>()
                .HasOne(s => s.TargetUser)
                .WithMany(t => t.LikedUser)
                .HasForeignKey(t => t.TargetUserId)
                .OnDelete(DeleteBehavior.Cascade);  //Might through error if you are using sql server //change it to NoAction

            builder.Entity<Message>()
                .HasOne(s => s.Sender)
                .WithMany(m => m.MessagesSent)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(s => s.Recipient)
                .WithMany(m => m.MessagesReceived)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
