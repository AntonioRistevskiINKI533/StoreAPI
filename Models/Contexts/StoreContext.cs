using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace StoreAPI.Models.Contexts
{
    public class StoreContext : DbContext
    {
        public StoreContext(DbContextOptions<StoreContext> options)
            :base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<DateSaleSumsView> DateSaleSumsView { get; set; }
        public DbSet<DatePurchaseSumsView> DatePurchaseSumsView { get; set; }
        public DbSet<ProductSaleSumsAndProfitView> ProductSaleSumsAndProfitView { get; set; }
        public DbSet<SupplierPurchaseSumsView> SupplierPurchaseSumsView { get; set; }
        public DbSet<ProductPurchaseSumsView> ProductPurchaseSumsView { get; set; }
        public DbSet<CitySaleSumsView> CitySaleSumsView { get; set; }
        public DbSet<CustomerSaleSumsView> CustomerSaleSumsView { get; set; }
        public DbSet<ProductTypeSaleSumsAndProfitView> ProductTypeSaleSumsAndProfitView { get; set; }
        public DbSet<BrandSaleSumsAndProfitView> BrandSaleSumsAndProfitView { get; set; }
        public DbSet<DayOfWeekSaleSumsView> DayOfWeekSaleSumsView { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Role> Role { get; set; }
    }
}
