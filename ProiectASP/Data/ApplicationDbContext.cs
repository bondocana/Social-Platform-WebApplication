using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProiectASP.Models;

namespace ProiectASP.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<ProfileGroup> ProfileGroups { get; set; }

        public DbSet<Chat> Chats { get; set; }

        public DbSet<Friend> Friends { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            // definire primary key compus
            modelBuilder.Entity<ProfileGroup>()
                .HasKey(ab => new { ab.Id, ab.ProfileId, ab.GroupId });


            // definire relatii cu modelele Group si Profile (FK)
            modelBuilder.Entity<ProfileGroup>()
                .HasOne(ab => ab.Profile)
                .WithMany(ab => ab.ProfileGroups)
                .HasForeignKey(ab => ab.ProfileId);

            modelBuilder.Entity<ProfileGroup>()
                .HasOne(ab => ab.Group)
                .WithMany(ab => ab.ProfileGroups)
                .HasForeignKey(ab => ab.GroupId);
        }

    }
}