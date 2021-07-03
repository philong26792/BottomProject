using Bottom_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Bottom_API.Data
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
        }

        public virtual DbSet<RoleUser> RoleUser { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<Materials> Materials { get; set; }
        public virtual DbSet<PoMaterials> PoMaterials { get; set; }
        public virtual DbSet<PoRoots> PoRoots { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RoleUser>().HasKey(x => x.Id);

            modelBuilder.Entity<Roles>().HasKey(x => x.Id);

            modelBuilder.Entity<Materials>().HasKey(x => x.Id);
            modelBuilder.Entity<PoMaterials>().HasKey(x => x.Id);
            modelBuilder.Entity<PoRoots>().HasKey(x => x.Id);
        }

    }
}