using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore;

namespace IMS.Plugins.EFCoreSql
{
    public class ProductTransactionRepositoryEF : IProductTransactionRepository
    {
        private readonly IDbContextFactory<IMSContext> _contextFactory;
        private readonly IProductRepository _productRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IInventoryTransactionRepository _inventoryTransactionRepository;

        public ProductTransactionRepositoryEF(IDbContextFactory<IMSContext> contextFactory, IProductRepository productRepository, IInventoryRepository inventoryRepository, IInventoryTransactionRepository inventoryTransactionRepository)
        {
            _contextFactory = contextFactory;
            _productRepository = productRepository;
            _inventoryRepository = inventoryRepository;
            _inventoryTransactionRepository = inventoryTransactionRepository;
        }

        public async Task ProduceProductAsync(string productionNumber, Product product, int quantity, string doneBy)
        {
            using var db = _contextFactory.CreateDbContext();
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

            db.ProductTransactions.Add(new ProductTransaction
            {
                ProductionNumber = productionNumber,
                ProductId = product.ProductId,
                QuantityBefore = product.Quantity,
                ActivityType = ProductTransactionType.ProduceProduct,
                QuantityAfter = product.Quantity + quantity,
                TransactionDate = DateTime.UtcNow,
                DoneBy = doneBy
            });

            await db.SaveChangesAsync();
        }

        public async Task SellProductAsync(string salesOrderNumber, Product product, int quantity, double price, string doneBy)
        {
            using var db = _contextFactory.CreateDbContext();

            db.ProductTransactions.Add(new ProductTransaction
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

            await db.SaveChangesAsync();
        }

        public async Task<IEnumerable<ProductTransaction>> GetProductTransactionsAsync(string productName, DateTime? dateFrom, DateTime? dateTo, ProductTransactionType? transactionType)
        {
            using var db = _contextFactory.CreateDbContext();

            var query = from pt in db.ProductTransactions
                        join prod in db.Products on pt.ProductId equals prod.ProductId
                        where (string.IsNullOrWhiteSpace(productName) || prod.ProductName.ToLower().IndexOf(productName.ToLower()) >= 0)
                        && (!dateFrom.HasValue || pt.TransactionDate >= dateFrom.Value.Date)
                        && (!dateTo.HasValue || pt.TransactionDate <= dateTo.Value.Date)
                        && (!transactionType.HasValue || pt.ActivityType == transactionType)
                        select pt;

            var result = await query.Include(q=>q.Product).ToListAsync();

            return result;
        }
    }
}
