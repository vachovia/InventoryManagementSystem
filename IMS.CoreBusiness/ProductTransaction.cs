using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMS.CoreBusiness
{
    public class ProductTransaction
    {
        public int ProductTransactionId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int QuantityBefore { get; set; }
        [Required]
        public ProductTransactionType ActivityType { get; set; }
        [Required]
        public int QuantityAfter { get; set; }
        public double? UnitPrice { get; set; }
        [Required]
        public DateTime TransactionDate { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(160)")]
        public string DoneBy { get; set; } = string.Empty;
        [Column(TypeName = "nvarchar(32)")]
        public string SONumber { get; set; } = string.Empty;
        [Column(TypeName = "nvarchar(32)")]
        public string ProductionNumber { get; set; } = string.Empty;
        public Product? Product { get; set; }
    }
}
