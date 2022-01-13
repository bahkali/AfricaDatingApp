using Microsoft.EntityFrameworkCore;
using ProfileService.Api.Domain.Entities;

namespace ProfileService.Api.Domain
{
    public class ProfileDataContext : DbContext
    {
        public ProfileDataContext(DbContextOptions options) : base(options)
        {}
        public DbSet<Photo> Photos { get; set; }

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<ProfileUser> ProfileUsers { get; set; }
    }
}