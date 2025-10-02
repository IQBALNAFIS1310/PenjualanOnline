using Microsoft.EntityFrameworkCore;
using PenjualanOnline.Models; // ganti sesuai namespace project kamu

namespace PenjualanOnline.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Account> Accounts { get; set; }
    }
}
