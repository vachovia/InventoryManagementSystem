using System.ComponentModel.DataAnnotations;

namespace IMS.CoreBusiness.Validations
{
    public  class Product_EnsurePriceIsGreaterThanInventoryCost: ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var product = validationContext.ObjectInstance as Product;

            if (product != null)
            {
                bool isValid = ValidatePricing(product);

                if (!isValid)
                {
                    var totalCost = TotalInventoriesCost(product);

                    return new ValidationResult($"The product's price is less than the inventories cost: {totalCost.ToString("c")}", new List<string> { validationContext.MemberName ?? string.Empty});
                }
            }

            return ValidationResult.Success;
        }

        private double TotalInventoriesCost(Product product)
        {
            if (product == null || product.ProductInventories == null)
            {
                return 0;
            }

            return product.ProductInventories.Sum(pi => pi.Inventory?.Price * pi.InventoryQuantity ?? 0);
        }

        private bool ValidatePricing(Product product)
        {
            if (product.ProductInventories == null || product.ProductInventories.Count <= 0)
            {
                return true;
            }

            var totalCost = TotalInventoriesCost(product);

            if (totalCost > product.Price)
            {
                return false;
            }

            return true;
        }
    }
}
