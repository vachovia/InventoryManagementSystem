using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMS.CoreBusiness
{
    public class InventoryTransaction
    {
        public int InventoryTransactionId { get; set; }
        [Required]
        public int InventoryId { get; set; }
        [Required]
        public int QuantityBefore { get; set; }
        [Required]
        public InventoryTransactionType ActivityType { get; set; }
        [Required]
        public int QuantityAfter { get; set; }
        public double UnitPrice { get; set; }
        [Required]
        public DateTime TransactionDate { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(160)")]
        public string DoneBy { get; set; } = string.Empty;
        [Column(TypeName = "nvarchar(32)")]
        public string PONumber { get; set; } = string.Empty;
        [Column(TypeName = "nvarchar(32)")]
        public string ProductionNumber { get; set; } = string.Empty;
        public  Inventory? Inventory { get; set; }
    }
}
