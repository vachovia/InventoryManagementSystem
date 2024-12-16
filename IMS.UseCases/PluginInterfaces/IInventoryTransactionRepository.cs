using IMS.CoreBusiness;

namespace IMS.UseCases.PluginInterfaces
{
    public interface IInventoryTransactionRepository
    {
        Task PurchaseInventoryAsync(string poNumber, Inventory inventory, int quantity, string doneBy, double price);
    }
}