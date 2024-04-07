using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TikTok_Clone_User_Service.Models;

namespace TikTok_Clone_User_Service.DatabaseContext
{
    public class DbUserContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Follower> Followers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=userDatabase.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
            .HasMany(user => user.Followers) // A user can have many followers
            .WithOne(follower => follower.User) // Each follower belongs to one user
            .HasForeignKey(follower => follower.UserId); // Foreign key relationship with UserId in Follower entity
        }

    }
}
