﻿using IMS.CoreBusiness;
using IMS.WebApp.ViewModelsValidations;
using System.ComponentModel.DataAnnotations;

namespace IMS.WebApp.ViewModels
{
    public class SellViewModel
    {
        [Required]
        public string SalesOrderNumber { get; set; } = string.Empty;

        [Range(minimum: 1, maximum: int.MaxValue, ErrorMessage = "You have to select a product.")]
        public int ProductId { get; set; }

        [Range(minimum: 1, maximum: int.MaxValue, ErrorMessage = "Quantity has to be greater or equal to 1.")]
        public int QuantityToSell { get; set; }

        [Range(minimum: 0, maximum: int.MaxValue, ErrorMessage = "Price has to be greater or equal to 0.")]
        [Sell_EnsureEnoughProductQunatity]
        public double UnitPrice { get; set; }

        public Product? Product { get; set; }
    }
}
