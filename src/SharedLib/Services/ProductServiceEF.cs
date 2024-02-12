using MongoDB.Driver;
using SharedLib.Interface;
using SharedLib.Models;

namespace SharedLib.Services
{
    public class ProductServiceEF(RetailDbContext retailDbContext) : IProductEf
    {
        private readonly RetailDbContext _retailDbContext = retailDbContext;

        public async Task<Product> AddProduct(Product product)
        {
            await _retailDbContext.Products.AddAsync(product);
            return product;
        }

        public async Task<List<Product>> GetAllProducts(bool featuredProducts = false, int pageSize = 0,
            int pageIndex = 0)
        {
            if (pageSize > 0)
            {
                return _retailDbContext.Products.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }

            return _retailDbContext.Products.ToList();
        }


        /// <summary>
        ///     Get List of products.
        /// </summary>
        public async Task<ProductCatalogResult> GetProductCatalogResult(bool featured = false, int pageSize = 0,
            int pageIndex = 0)
        {
            try
            {
                var products = _retailDbContext.DisplayProducts
                    .Where(_ => _.featuredProduct == true).Skip(pageIndex * pageSize).Take(pageSize).ToList();

                var plist = products.ConvertAll(ConvertProduct);

                var count = products.Count();

                var result = new ProductCatalogResult(pageIndex, pageSize, count, plist);

                return result;
            }
            catch (MongoException ex)
            {
                var e = ex;
                throw;
            }
        }


        public async Task<Product> UpdateProduct(Product product)
        {
            _retailDbContext.Products.Update(product);
            return product;
        }

        public async Task DeleteProduct(Product product)
        {
            _retailDbContext.Products.Remove(product);
        }

        public static Product ConvertProduct(DisplayProduct v)
        {
            return new Product
            {
                id = v.id,
                categoryId = v.categoryId,
                categoryName = v.categoryName,
                sku = v.sku,
                name = v.name,
                description = v.description,
                price = v.price,
                tags = v.tags,
                featuredProduct = v.featuredProduct,
                imageUrl = v.imageUrl,
                image = v.image
            };
        }
    }
}