using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;
using Microsoft.EntityFrameworkCore;

namespace IMS.Plugins.EFCoreSql
{
    public class InventoryTransactionRepositoryEF : IInventoryTransactionRepository
    {
        private readonly IDbContextFactory<IMSContext> _contextFactory;

        public InventoryTransactionRepositoryEF(IDbContextFactory<IMSContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IEnumerable<InventoryTransaction>> GetInventoryTransactionsAsync(string inventoryName, DateTime? dateFrom, DateTime? dateTo, InventoryTransactionType? transactionType)
        {
            using var db = _contextFactory.CreateDbContext();

            var query = from it in db.InventoryTransactions
                        join inv in db.Inventories on it.InventoryId equals inv.InventoryId
                        where (string.IsNullOrWhiteSpace(inventoryName) || inv.InventoryName.ToLower().IndexOf(inventoryName.ToLower()) >= 0)
                        && (!dateFrom.HasValue || it.TransactionDate >= dateFrom.Value.Date)
                        && (!dateTo.HasValue || it.TransactionDate <= dateTo.Value.Date)
                        && (!transactionType.HasValue || it.ActivityType == transactionType)
                        select it;

            var result = await query.Include(q => q.Inventory).ToListAsync();

            return result;
        }

        public async Task ProduceInventoryAsync(string productionNumber, Inventory inventory, int quantityToConsume, string doneBy, double price)
        {
            using var db = _contextFactory.CreateDbContext();

            db.InventoryTransactions.Add(new InventoryTransaction
            {
                ProductionNumber = productionNumber,
                InventoryId = inventory.InventoryId,
                QuantityBefore = inventory.Quantity,
                QuantityAfter = inventory.Quantity - quantityToConsume,
                ActivityType = InventoryTransactionType.ProduceProduct,
                TransactionDate = DateTime.UtcNow,
                DoneBy = doneBy,
                UnitPrice = price
            });

            await db.SaveChangesAsync();
        }

        public async Task PurchaseInventoryAsync(string poNumber, Inventory inventory, int quantity, string doneBy, double price)
        {
            using var db = _contextFactory.CreateDbContext();

            db.InventoryTransactions.Add(new InventoryTransaction
            {
                PONumber = poNumber,
                InventoryId = inventory.InventoryId,
                QuantityBefore = inventory.Quantity,
                QuantityAfter = inventory.Quantity + quantity,
                ActivityType = InventoryTransactionType.PurchaseInventory,
                TransactionDate = DateTime.UtcNow,
                DoneBy = doneBy,
                UnitPrice = price
            });

            await db.SaveChangesAsync();
        }
    }
}
