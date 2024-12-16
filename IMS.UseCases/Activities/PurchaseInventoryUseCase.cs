using IMS.CoreBusiness;
using IMS.UseCases.Activities.Interfaces;
using IMS.UseCases.PluginInterfaces;

namespace IMS.UseCases.Activities
{
    public class PurchaseInventoryUseCase: IPurchaseInventoryUseCase
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IInventoryTransactionRepository _inventoryTransactionRepository;        

        public PurchaseInventoryUseCase(IInventoryTransactionRepository inventoryTransactionRepository, IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
            _inventoryTransactionRepository = inventoryTransactionRepository;
        }

        public async Task ExecuteAsync(string poNumber, Inventory inventory, int quantity, string doneBy)
        {
            // Insert record into the table
            await _inventoryTransactionRepository.PurchaseInventoryAsync(poNumber, inventory, quantity, doneBy, inventory.Price);

            // Increase the quantity and update inventory
            inventory.Quantity += quantity;

            await _inventoryRepository.UpdateInventoryAsync(inventory);
        }
    }
}
