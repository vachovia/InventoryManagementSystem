using IMS.CoreBusiness;

namespace IMS.UseCases.PluginInterfaces
{
    public interface IInventoryTransactionRepository
    {
        Task PurchaseInventoryAsync(string poNumber, Inventory inventory, int quantity, string doneBy, double price);

        Task ProduceInventoryAsync(string poNumber, Inventory inventory, int quantityToConsume, string doneBy, double price);
    }
}