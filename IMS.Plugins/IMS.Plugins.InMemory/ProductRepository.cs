using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;

namespace IMS.Plugins.InMemory
{
    public class ProductRepository : IProductRepository
    {
        private List<Product> _products;

        public ProductRepository()
        {
            _products = new List<Product>
            {
                new Product { ProductId = 1, ProductName = "Bike", Price = 150, Quantity = 25 },
                new Product { ProductId = 2, ProductName = "Car", Price = 25000, Quantity = 200 },
            };
        }

        public async Task<IEnumerable<Product>> GetProductsByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return await Task.FromResult(_products);
            }

            return _products.Where(
                i => i.ProductName.Contains(name, StringComparison.OrdinalIgnoreCase)
            );
        }

        public Task AddProductAsync(Product product)
        {
            var found = _products.Any(i => i.ProductName.Equals(product.ProductName, StringComparison.OrdinalIgnoreCase));

            if (found)
            {
                return Task.CompletedTask;
            }

            var maxId = _products.Max(i => i.ProductId);

            product.ProductId = maxId + 1;

            _products.Add(product);

            return Task.CompletedTask;
        }

        public Task UpdateProductAsync(Product product)
        {
            var found = _products.Any(i => i.ProductId != product.ProductId && i.ProductName.Equals(product.ProductName, StringComparison.OrdinalIgnoreCase));

            if (found)
            {
                return Task.CompletedTask;
            }

            var prodToUpdate = _products.FirstOrDefault(i => i.ProductId == product.ProductId);

            if (prodToUpdate is not null)
            {
                prodToUpdate.ProductName = product.ProductName;
                prodToUpdate.Quantity = product.Quantity;
                prodToUpdate.Price = product.Price;
            }

            return Task.CompletedTask;
        }

        public Task<Product> GetProductByIdAsync(int productId)
        {
            var product = _products.First(i => i.ProductId == productId);

            return Task.FromResult(product);
        }

        public Task DeleteProductByIdAsync(int productId)
        {
            var product = _products.FirstOrDefault(i => i.ProductId == productId);

            if (product != null)
            {
                _products.Remove(product);
            }

            return Task.CompletedTask;
        }
    }
}
