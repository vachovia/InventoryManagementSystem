﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMS.CoreBusiness
{
    public class Inventory
    {
        public int InventoryId { get; set; }

        [Required]
        [StringLength(150)]
        [Column(TypeName = "nvarchar(160)")]
        public string InventoryName { get; set; } = string.Empty;

        [Range(0,int.MaxValue, ErrorMessage ="Quantity must be greator or equal to {1}")]
        public int Quantity { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Price must be greator or equal to {1}")]
        public double Price { get; set; }

        public List<ProductInventory> ProductInventories { get; set; } = new();
    }
}
