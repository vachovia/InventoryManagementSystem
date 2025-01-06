using IMS.CoreBusiness;
using Microsoft.EntityFrameworkCore;

namespace IMS.Plugins.EFCoreSql
{
    public class IMSContext: DbContext
    {
        public IMSContext(DbContextOptions<IMSContext> options): base(options) { }

        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductInventory> ProductInventories { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
        public DbSet<ProductTransaction> ProductTransactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Hover over HasKey, HasOne, WithMany, HasForeignKey functions and it is self-explanatory:
            modelBuilder.Entity<ProductInventory>().HasKey(pi => new { pi.ProductId, pi.InventoryId });

            modelBuilder.Entity<ProductInventory>().HasOne(pi => pi.Product).WithMany(p => p.ProductInventories).HasForeignKey(pi => pi.ProductId);

            modelBuilder.Entity<ProductInventory>().HasOne(pi => pi.Inventory).WithMany(i => i.ProductInventories).HasForeignKey(pi => pi.InventoryId);

            // Seed Data:
            modelBuilder.Entity<Inventory>().HasData(
                new Inventory { InventoryId = 1, InventoryName = "Bike Seat", Price = 2, Quantity = 10 },
                new Inventory { InventoryId = 2, InventoryName = "Bike Body", Price = 15, Quantity = 10 },
                new Inventory { InventoryId = 3, InventoryName = "Bike Wheel", Price = 8, Quantity = 20 },
                new Inventory { InventoryId = 4, InventoryName = "Bike Pedal", Price = 1, Quantity = 20 }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product { ProductId = 1, ProductName = "Bike", Price = 150, Quantity = 10 },
                new Product { ProductId = 2, ProductName = "Car", Price = 25000, Quantity = 5 }
            );

            modelBuilder.Entity<ProductInventory>().HasData(
                new ProductInventory { ProductId = 1, InventoryId = 1, InventoryQuantity = 1 },
                new ProductInventory { ProductId = 1, InventoryId = 2, InventoryQuantity = 1 },
                new ProductInventory { ProductId = 1, InventoryId = 3, InventoryQuantity = 2 },
                new ProductInventory { ProductId = 1, InventoryId = 4, InventoryQuantity = 2 }
            );
        }
    }
}
