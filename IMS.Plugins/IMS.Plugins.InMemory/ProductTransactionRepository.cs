using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;
using System.Diagnostics;

namespace IMS.Plugins.InMemory
{
    public class ProductTransactionRepository : IProductTransactionRepository
    {
        private List<ProductTransaction> _productTransactions = new();

        private readonly IProductRepository _productRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IInventoryTransactionRepository _inventoryTransactionRepository;

        public ProductTransactionRepository(IProductRepository productRepository, IInventoryRepository inventoryRepository, IInventoryTransactionRepository inventoryTransactionRepository)
        {
            _productRepository = productRepository;
            _inventoryRepository = inventoryRepository;
            _inventoryTransactionRepository = inventoryTransactionRepository;
        }

        public async Task ProduceProductAsync(string productionNumber, Product product, int quantity, string doneBy)
        {
            var prod = await _productRepository.GetProductByIdAsync(product.ProductId);

            if (prod != null)
            {
                foreach (var pi in prod.ProductInventories)
                {
                    if (pi.Inventory is not null)
                    {
                        await _inventoryTransactionRepository.ProduceInventoryAsync(productionNumber, pi.Inventory, pi.InventoryQuantity * quantity, doneBy, -1);

                        var inv = await _inventoryRepository.GetInventoryByIdAsync(pi.InventoryId);
                        inv.Quantity -= pi.InventoryQuantity * quantity;
                        await _inventoryRepository.UpdateInventoryAsync(inv);
                    }                    
                }
            }

            _productTransactions.Add(new ProductTransaction
            {
                ProductionNumber = productionNumber,
                ProductId = product.ProductId,
                QuantityBefore = product.Quantity,
                ActivityType = ProductTransactionType.ProduceProduct,
                QuantityAfter = product.Quantity + quantity,
                TransactionDate = DateTime.UtcNow,
                DoneBy = doneBy
            });
        }

        public Task SellProductAsync(string salesOrderNumber, Product product, int quantity, double price, string doneBy)
        {
            _productTransactions.Add(new ProductTransaction
            {
                ActivityType = ProductTransactionType.SellProduct,
                SONumber = salesOrderNumber,
                ProductId = product.ProductId,
                QuantityBefore = product.Quantity,
                QuantityAfter = product.Quantity - quantity,
                TransactionDate = DateTime.UtcNow,
                DoneBy = doneBy,
                UnitPrice = price
            });

            return Task.CompletedTask;
        }

        public async Task<IEnumerable<ProductTransaction>> GetProductTransactionsAsync(string productName, DateTime? dateFrom, DateTime? dateTo, ProductTransactionType? transactionType)
        {
            var products = (await _productRepository.GetProductsByNameAsync(string.Empty)).ToList();

            var query = from p in _productTransactions
                        join prod in products on p.ProductId equals prod.ProductId
                        where (string.IsNullOrWhiteSpace(productName) || prod.ProductName.ToLower().IndexOf(productName.ToLower()) >= 0)
                        && (!dateFrom.HasValue || p.TransactionDate >= dateFrom.Value.Date)
                        && (!dateTo.HasValue || p.TransactionDate <= dateTo.Value.Date)
                        && (!transactionType.HasValue || p.ActivityType == transactionType)
                        select new ProductTransaction
                        {
                            Product = prod,
                            ProductTransactionId = p.ProductTransactionId,
                            ProductionNumber = p.ProductionNumber,
                            SONumber = p.SONumber,
                            ProductId = p.ProductId,
                            QuantityBefore = p.QuantityBefore,
                            ActivityType = p.ActivityType,
                            QuantityAfter = p.QuantityAfter,
                            TransactionDate = p.TransactionDate,
                            DoneBy = p.DoneBy,
                            UnitPrice = p.UnitPrice
                        };
            return query;
        }
    }
}
