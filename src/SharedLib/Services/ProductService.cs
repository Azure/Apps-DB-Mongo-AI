using SharedLib.Interface;
using SharedLib.Models;

namespace SharedLib.Services
{
    public class ProductService : IProduct
    {
        public MongoDbService _mongoDbService;

        public ProductService(MongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }

        public async Task<Product> AddProduct(Product product)
        {
            return await _mongoDbService.UpsertProductAsync(product);
        }

        public async Task<List<Product>> GetAllProducts(bool featuredProducts = false, int pageSize = 0,
            int pageIndex = 0)
        {
            if (pageSize > 0)
            {
                return await _mongoDbService.GetProductsAsync(featuredProducts, pageSize, pageIndex);
            }

            return await _mongoDbService.GetProductsAsync(featuredProducts);
        }

        public async Task<Product> UpdateProduct(Product product)
        {
            return await _mongoDbService.UpsertProductAsync(product);
        }

        public async Task DeleteProduct(Product product)
        {
            await _mongoDbService.DeleteProductAsync(product);
        }

        public async Task<ProductCatalogResult> GetProductCatalogResult(bool featuredProducts = false, int pageSize = 0,
            int pageIndex = 0)
        {
            return await _mongoDbService.GetProductCatalogResult(featuredProducts, pageSize, pageIndex);
        }
    }
}