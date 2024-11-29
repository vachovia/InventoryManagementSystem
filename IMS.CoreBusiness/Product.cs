using IMS.CoreBusiness.Validations;
using System.ComponentModel.DataAnnotations;

namespace IMS.CoreBusiness
{
    public class Product
    {
        public int ProductId { get; set; }

        [Required]
        [StringLength(150)]
        public string ProductName { get; set; } = string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be greator or equal to {1}")]
        public int Quantity { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Price must be greator or equal to {1}")]
        public double Price { get; set; }

        [Product_EnsurePriceIsGreaterThanInventoryCost]
        public List<ProductInventory> ProductInventories { get; set; } = new();

        public void AddInventory(Inventory inventory)
        {
            bool exists = ProductInventories.Any(x => x.Inventory is not null && x.Inventory.InventoryName.Equals(inventory.InventoryName, StringComparison.OrdinalIgnoreCase));

            if (!exists)
            {
                ProductInventories.Add(new ProductInventory
                {
                    InventoryId = inventory.InventoryId,
                    Inventory = inventory,
                    InventoryQuantity = 1,
                    ProductId = this.ProductId,
                    Product = this
                });
            }            
        }

        public void RemoveInventory(ProductInventory prodInventory)
        {
            ProductInventories.Remove(prodInventory);
        }
    }
}