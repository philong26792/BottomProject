using Bottom_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Bottom_API.Data
{
    public class HPDataContext : DbContext
    {
        public HPDataContext(DbContextOptions<HPDataContext> options) : base(options) { }
        public DbSet<HP_Vendor_u01> HP_Vendor_u01 { get; set; }
        public DbSet<HP_Upload_Time_ie27_1_log> HP_Upload_Time_ie27_1_log { get; set; }
        public DbSet<HP_Holidays_i46> HP_Holidays_i46 { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HP_Upload_Time_ie27_1_log>().HasKey(x => new { x.Factory_ID, x.Version });
            modelBuilder.Entity<HP_Holidays_i46>().HasKey(x => new { x.Factory_ID, x.Holiday, x.Division_No, x.Company_No });
        }
    }
}