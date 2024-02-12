using SharedLib.Models;

namespace SharedLib.Interface
{
    public interface IProductEf
    {
        Task<Product> AddProduct(Product product);
        Task DeleteProduct(Product product);
        Task<List<Product>> GetAllProducts(bool featuredProducts = false, int pageSize = 0, int pageIndex = 0);
        Task<Product> UpdateProduct(Product product);
        Task<ProductCatalogResult> GetProductCatalogResult(bool featured = false, int pageSize = 0, int pageIndex = 0);
    }
}