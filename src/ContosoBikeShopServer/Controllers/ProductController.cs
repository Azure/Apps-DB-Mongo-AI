using Microsoft.AspNetCore.Mvc;
using SharedLib.Interface;
using SharedLib.Models;
using SharedLib.Services;

namespace ContosoBikeShopServer.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductController(ProductService productService, IProductEf productServiceEf) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> AddProduct(Product product)
        {
            if (product == null)
            {
                return BadRequest("Product is not valid");
            }

            return Ok(await productService.AddProduct(product));
        }

        [HttpPost]
        public async Task DeleteProduct(Product product)
        {
            await productService.DeleteProduct(product);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts(bool featuredProducts = false, int pageSize = 0,
            int pageIndex = 0)
        {
            return Ok(await productService.GetAllProducts(featuredProducts, pageSize, pageIndex));
        }

        [HttpGet]
        public async Task<IActionResult> GetProductCatalogResult(bool featuredProducts = false, int pageSize = 0,
            int pageIndex = 0)
        {
            return Ok(await productService.GetProductCatalogResult(featuredProducts, pageSize, pageIndex));
        }

        [HttpGet]
        public async Task<IActionResult> GetProductCatalogResultEF(bool featuredProducts = false, int pageSize = 0,
            int pageIndex = 0)
        {
            return Ok(await productServiceEf.GetProductCatalogResult(featuredProducts, pageSize, pageIndex));
        }

        [HttpPost]
        public async Task<Product> UpdateProduct(Product product)
        {
            return await productService.UpdateProduct(product);
        }
    }
}