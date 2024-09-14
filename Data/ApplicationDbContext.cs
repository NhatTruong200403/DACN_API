using GoWheels_WebAPI.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GoWheels_WebAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            
        }
        public DbSet<Amentity> Amentities { get; set; }
        public DbSet<CarType> CarTypes { get; set; }
        public DbSet<CarTypeDetail> CarTypeDetails { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<PromotionType> PromotionTypes { get; set; }
    }
}
