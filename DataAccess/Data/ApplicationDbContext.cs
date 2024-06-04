using Microsoft.EntityFrameworkCore;
using Models.Models;
using SmartBillingServer.Models;

namespace SmartBillingServer.DataAccess.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Action", DisplayOrder = 1 },
                new Category { Id = 2, Name = "SciFi", DisplayOrder = 2 },
                new Category { Id = 3, Name = "History", DisplayOrder = 3 }
                );

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1,Barcode="101", Title = "Saree", Description = "", Quantity = 1, CostPrice = 300, SellPrice = 500, CategoryId = 1 },
                new Product { Id = 2, Barcode="102", Title = "Jeans", Description = "", Quantity = 1, CostPrice = 300, SellPrice = 1500, CategoryId = 2 },
                new Product { Id = 3, Barcode="103", Title = "Shirt", Description = "", Quantity = 1, CostPrice = 300, SellPrice = 400, CategoryId = 2 },
                new Product { Id = 4, Barcode="104", Title = "Socks", Description = "", Quantity = 1, CostPrice = 300, SellPrice = 150, CategoryId = 1 },
                new Product { Id = 5, Barcode="105", Title = "Lungi", Description = "", Quantity = 1, CostPrice = 300, SellPrice = 80, CategoryId = 3 },
                new Product { Id = 6, Barcode="106", Title = "Frock", Description = "", Quantity = 1, CostPrice = 300, SellPrice = 345, CategoryId = 1 },
                new Product { Id = 7, Barcode="107", Title = "Lengha", Description = "", Quantity = 1, CostPrice = 300, SellPrice = 800, CategoryId = 3 },
                new Product { Id = 8, Barcode="108", Title = "Gamcha", Description = "", Quantity = 1, CostPrice = 300, SellPrice = 120, CategoryId = 1 },
                new Product { Id = 9, Barcode="109", Title = "Shoes", Description = "", Quantity = 1, CostPrice = 300, SellPrice = 700, CategoryId = 2 },
                new Product { Id = 10, Barcode="110", Title = "Cap", Description = "", Quantity = 1, CostPrice = 300, SellPrice = 50, CategoryId = 1 }
                );

            //var billItems = new List<BillItem>() {
            //    new BillItem(0, "101","Shirt",1,500, 0, 500),
            //    new BillItem(0, "102","T-Shirt",1,500, 0, 500),
            //    new BillItem(0, "103","Jeans",1,500, 0, 500),
            //    new BillItem(0, "104","T-Shirt",1,500, 0, 500)
            //};

            //modelBuilder.Entity<BillItem>().HasData(
            //        billItems
            //    );

            modelBuilder.Entity<Bill>().HasData(
                new Bill
                {
                    Id = 1,
                    ModeOfPayment = "Cash",
                    SubTotal = 2000,
                    DiscountAmount = 0,
                    TotalAmount = 2000,
                    CustomerName = "default",
                    CustomerAddress = "NA"
                });

            modelBuilder.Entity<BillItem>().HasData(
                new BillItem() { BillId = 1, Id = 1, ItemId = 0, Barcode = "101", ItemName = "Shirt", Quantity = 1, Price = 500, DiscountAmount = 0, Amount = 500 },
                new BillItem() { BillId = 1, Id = 2, ItemId = 0, Barcode = "102", ItemName = "T-Shirt", Quantity = 1, Price = 500, DiscountAmount = 0, Amount = 500 },
                new BillItem() { BillId = 1, Id = 3, ItemId = 0, Barcode = "103", ItemName = "Jeans", Quantity = 1, Price = 500, DiscountAmount = 0, Amount = 500 },
                new BillItem() { BillId = 1, Id = 4, ItemId = 0, Barcode = "104", ItemName = "T-Shirt", Quantity = 1, Price = 500, DiscountAmount = 0, Amount = 500 }
                );
        }
    }
}
