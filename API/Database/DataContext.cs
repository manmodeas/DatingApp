using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace API.Database
{
    public class DataContext(DbContextOptions options) : IdentityDbContext<AppUser, AppRole, int, 
        IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, 
        IdentityUserToken<int>>(options)
    {
        public DbSet<UserLike> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Connection> Connections { get; set; }

             
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.User)
                .HasForeignKey(k => k.UserId)
                .IsRequired();

            builder.Entity<AppRole>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.Role)
                .HasForeignKey(k => k.RoleId)
                .IsRequired();

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

            var utcConverter = new ValueConverter<DateTime, DateTime>(
               v => v, // Save as-is
               v => DateTime.SpecifyKind(v, DateTimeKind.Utc)); // Read as UTC

            var utcConverter2 = new ValueConverter<DateTime?, DateTime?>(
               v => v, // Save as-is
               v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : null); // Read as UTC


            builder.Entity<Message>()
                .Property(e => e.MessageSent)
                .HasConversion(utcConverter);

            builder.Entity<Message>()
                .Property(e => e.DataRead)
                .HasConversion(utcConverter2);
        }
    }
}
