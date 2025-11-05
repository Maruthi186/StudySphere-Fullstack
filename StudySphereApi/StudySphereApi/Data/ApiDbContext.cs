using Microsoft.EntityFrameworkCore;
using StudySphereApi.Models; // <-- This is the namespace for our models
using System.Collections.Generic; // This line might not be needed here, but doesn't hurt

namespace StudySphereApi.Data
{
    // This class is the "bridge"
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {
        }

        // --- Our Database Tables ---
        public DbSet<Group> Groups { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }


        // This method is called by EF Core when it's building the database model
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // --- Configure the 'UserGroup' junction table ---

            // 1. Tell EF Core to use a "Composite Key"
            // This means a UserID + GroupID pair must be unique.
            // A user can't join the same group twice.
            modelBuilder.Entity<UserGroup>()
                .HasKey(ug => new { ug.UserId, ug.GroupId });

            // 2. Define the relationship: One User has many UserGroups
            modelBuilder.Entity<UserGroup>()
                .HasOne(ug => ug.User)
                .WithMany(u => u.UserGroups)
                .HasForeignKey(ug => ug.UserId);

            // 3. Define the relationship: One Group has many UserGroups
            modelBuilder.Entity<UserGroup>()
                .HasOne(ug => ug.Group)
                .WithMany() // A Group doesn't *need* a list of UserGroups, so we leave this blank
                .HasForeignKey(ug => ug.GroupId);
        }
    }
}