using IMS.WebApp.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace IMS.WebApp.ViewModelsValidations
{
    public class Sell_EnsureEnoughProductQunatity: ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var sellViewModel = validationContext.ObjectInstance as SellViewModel;

            if (sellViewModel is not null)
            {
                var product = sellViewModel.Product;

                if (product != null && product.Quantity < sellViewModel.QuantityToSell)
                {
                    return new ValidationResult($"There isn't enough product. There is only {product.Quantity} in warehouse.", [validationContext.MemberName ?? ""]);
                }
            }

            return ValidationResult.Success;
        }
    }
}
