using BookShop.MVC.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookShop.MVC.Data
{
    public class ApplicationDbContext : IdentityDbContext<ShopUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<OrderInfo> OrderInfos { get; set; }
        //public DbSet<CartItem> CartItems { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    // Настроить отношение один ко многим между OrderInfo и CartItem
        //    modelBuilder.Entity<CartItem>()
        //        .HasOne(c => c.OrderInfo)
        //        .WithMany(o => o.CartItems)
        //        .HasForeignKey(c => c.OrderInfoId);
        //}
    }
}
