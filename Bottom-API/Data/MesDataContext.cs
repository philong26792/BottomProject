using Bottom_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Bottom_API.Data
{
    public class MesDataContext : DbContext
    {
        public MesDataContext(DbContextOptions<MesDataContext> options) : base(options) { }
        public DbSet<MES_MO> MES_MO {get;set;}
        public DbSet<MES_MO_Size> MES_MO_Size {get;set;}
        public DbSet<MES_MO_Basic> MES_MO_Basic {get;set;}
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<MES_MO>().HasKey(x => new {x.Factory_ID, x.Cycle_No});
            modelBuilder.Entity<MES_MO_Size>().HasKey(e => new { e.Cycle_No, e.Size_Code, e.Factory_ID });
            modelBuilder.Entity<MES_MO_Basic>().HasKey(e => new { e.MO_No, e.MO_Seq, e.Factory_ID });
        }
    }
}