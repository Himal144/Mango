using Mango.Services.ProductAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ProductAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        //Passing the options to the base class DbContext of the ApplicationDbContext
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options) 
        { 
        }
        
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    ProductId = 1,
                    Name = "Laptop",
                    Price = 100000,
                    Description="Best laptop under 100000",
                    CategoryName ="Electronic Gadgets",
                    ImageUrl= "https://w7.pngwing.com/pngs/723/514/png-transparent-laptop-personal-computer-laptops-electronics-photography-computer-thumbnail.png"
                });

            modelBuilder.Entity<Product>().HasData(
               new Product
               {
                   ProductId = 2,
                   Name = "Laptop",
                   Price = 100000,
                   Description = "Best laptop under 100000",
                   CategoryName = "Electronic Gadgets",
                   ImageUrl = "https://w7.pngwing.com/pngs/723/514/png-transparent-laptop-personal-computer-laptops-electronics-photography-computer-thumbnail.png"
               });
            modelBuilder.Entity<Product>().HasData(
               new Product
               {
                   ProductId = 3,
                   Name = "Laptop",
                   Price = 100000,
                   Description = "Best laptop under 100000",
                   CategoryName = "Electronic Gadgets",
                   ImageUrl = "https://w7.pngwing.com/pngs/723/514/png-transparent-laptop-personal-computer-laptops-electronics-photography-computer-thumbnail.png"
               });
        }
    }
}
