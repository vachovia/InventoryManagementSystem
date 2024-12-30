using IMS.WebApp.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace IMS.WebApp.ViewModelsValidations
{
    public class Produce_EnsureEnoughInventoryQuantity: ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var produceViewModel = validationContext.ObjectInstance as ProduceViewModel;

            if(produceViewModel is not null)
            {
                var product = produceViewModel.Product;

                if (product != null && product.ProductInventories != null)
                {
                    foreach (var pi in product.ProductInventories)
                    {
                        var numberOfInventories = pi.InventoryQuantity * produceViewModel.QuantityToProduce;

                        if (pi.Inventory != null && numberOfInventories > pi.Inventory.Quantity)
                        {
                            return new ValidationResult($"The inventory {pi.Inventory.InventoryName} is not enough to produce {produceViewModel.QuantityToProduce} products.", [validationContext.MemberName ?? ""]);
                        }
                    }
                }
            }

            return ValidationResult.Success;
        }
    }
}
