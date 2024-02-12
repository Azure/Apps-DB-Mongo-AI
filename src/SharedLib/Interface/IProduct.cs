using SharedLib.Models;

namespace SharedLib.Interface
{
    public interface IProduct
    {
        public Task<Product> AddProduct(Product product);
        public Task DeleteProduct(Product product);
        public Task<List<Product>> GetAllProducts(bool featuredProducts = false, int pageSize = 0, int pageIndex = 0);
        public Task<Product> UpdateProduct(Product product);
    }
}