using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;

namespace IMS.Plugins.InMemory
{
    public class InventoryRepository : IInventoryRepository
    {
        private List<Inventory> _inventories;

        public InventoryRepository()
        {
            _inventories = new List<Inventory>
            {
                new Inventory { InventoryId = 1, InventoryName = "Bike Seat", Price = 2, Quantity = 10 },
                new Inventory { InventoryId = 2, InventoryName = "Bike Body", Price = 15, Quantity = 10 },
                new Inventory { InventoryId = 3, InventoryName = "Bike Wheels", Price = 8, Quantity = 20 },
                new Inventory { InventoryId = 4, InventoryName = "Bike Pedals", Price = 1, Quantity = 20 },
            };
        }

        public async Task<IEnumerable<Inventory>> GetInventoriesByNameAsync(string name)
        {
            if(string.IsNullOrWhiteSpace(name))
            {
                return await Task.FromResult(_inventories);
            }

            return _inventories.Where(
                i => i.InventoryName.Contains(name, StringComparison.OrdinalIgnoreCase)
            );
        }

        public Task AddInventoryAsync(Inventory inventory)
        {
            var found = _inventories.Any(i => i.InventoryName.Equals(inventory.InventoryName, StringComparison.OrdinalIgnoreCase));

            if (found)
            {
                return Task.CompletedTask;
            }

            var maxId = _inventories.Max(i => i.InventoryId);

            inventory.InventoryId = maxId + 1;

            _inventories.Add(inventory);

            return Task.CompletedTask;
        }

        public Task UpdateInventoryAsync(Inventory inventory)
        {
            var found = _inventories.Any(i => i.InventoryId != inventory.InventoryId && i.InventoryName.Equals(inventory.InventoryName, StringComparison.OrdinalIgnoreCase));

            if (found)
            {
                return Task.CompletedTask;
            }

            var invToUpdate = _inventories.FirstOrDefault(i => i.InventoryId == inventory.InventoryId);

            if (invToUpdate is not null)
            {
                invToUpdate.InventoryName = inventory.InventoryName;
                invToUpdate.Quantity = inventory.Quantity;
                invToUpdate.Price = inventory.Price;
            }

            return Task.CompletedTask;
        }

        public Task<Inventory> GetInventoryByIdAsync(int inventoryId)
        {
            var inventory = _inventories.First(i => i.InventoryId == inventoryId);

            var newInventory = new Inventory
            {
                InventoryId = inventory.InventoryId,
                InventoryName = inventory.InventoryName,
                Quantity = inventory.Quantity,
                Price = inventory.Price                
            };

            return Task.FromResult(newInventory);
        }

        public Task DeleteInventoryByIdAsync(int inventoryId)
        {
            var inventory = _inventories.FirstOrDefault(i => i.InventoryId == inventoryId);

            if(inventory != null)
            {
                _inventories.Remove(inventory);
            }

            return Task.CompletedTask;
        }
    }
}
