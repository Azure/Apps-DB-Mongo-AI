using System.Net.Http.Json;
using SharedLib.Interface;
using SharedLib.Models;

namespace ContosoBikeShopWebApp.Services
{
    public class ProductClientService(IHttpClientFactory clientFactory) : IProduct
    {
        private readonly HttpClient httpClient = clientFactory.CreateClient("WebApiClient");

        public async Task<Product> AddProduct(Product product)
        {
            var content = ApiHelper.GenerateStringContent(ApiHelper.SerializeObj(product));
            var response = await httpClient.PostAsync("api/Product/AddProduct", content);
            var result = await ApiHelper.ReadContent(response);
            return (Product)ApiHelper.DeserializeJsonStringList<Product>(result)!;
        }

        public Task DeleteProduct(Product product)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Product>> GetAllProducts(bool featured = false, int pageSize = 0, int pageIndex = 0)
        {
            var response =
                await httpClient.GetAsync(
                    $"api/Product/GetAllProducts?pageIndex={pageIndex}&pageSize={pageSize}&featured={featured}");

            var result = await ApiHelper.ReadContent(response);
            return (List<Product>?)ApiHelper.DeserializeJsonStringList<Product>(result)!;
        }

        public async Task<Product> UpdateProduct(Product product)
        {
            var content = ApiHelper.GenerateStringContent(ApiHelper.SerializeObj(product));
            var response = await httpClient.PostAsync("api/Product/UpdateProduct", content);
            var result = await ApiHelper.ReadContent(response);
            return ApiHelper.DeserializeJsonString<Product>(result);
        }

        public async Task<ProductCatalogResult> GetProductCatalogResult(bool featured = false, int pageSize = 0,
            int pageIndex = 0)
        {
            var response = await httpClient.GetFromJsonAsync<ProductCatalogResult>(
                $"api/Product/GetProductCatalogResult?pageIndex={pageIndex}&pageSize={pageSize}&featured={featured}");
            response.Data.ForEach(_ =>
                _.imageUrl = string.IsNullOrEmpty(_.imageUrl)
                    ? "images/bikes/no-image.png"
                    : $"images/bikes/{_.imageUrl}");
            return response;
        }

        public record ProductResults(int PageIndex, int PageSize, bool Featured);
    }
}