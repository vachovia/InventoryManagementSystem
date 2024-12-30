using IMS.CoreBusiness;
using IMS.UseCases.Activities.Interfaces;
using IMS.UseCases.PluginInterfaces;

namespace IMS.UseCases.Activities
{
    public class SellProductUseCase : ISellProductUseCase
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductTransactionRepository _productTransactionRepository;        

        public SellProductUseCase(IProductTransactionRepository productTransactionRepository, IProductRepository productRepository)
        {
            _productRepository = productRepository;
            _productTransactionRepository = productTransactionRepository;
        }

        public async Task ExecuteAsync(string salesOrderNumber, Product product, int quantity, double price, string doneBy)
        {
            await _productTransactionRepository.SellProductAsync(salesOrderNumber, product, quantity, price, doneBy);

            product.Quantity -= quantity; 

            await _productRepository.UpdateProductAsync(product);
        }
    }
}
