using System.Net.Http.Json;
using SharedLib.Models;

namespace ContosoBikeShopWebApp.Services
{
    public class CustomerClientService(IHttpClientFactory clientFactory)
    {
        private readonly HttpClient httpClient = clientFactory.CreateClient("WebApiClient");

        public async Task AddCustomer(Customer customer)
        {
            var content = ApiHelper.GenerateStringContent(ApiHelper.SerializeObj(customer));
            var response = await httpClient.PostAsync("api/Customer/AddCustomer", content);
        }

        public async Task DeleteCustomer(Customer customer)
        {
            var content = ApiHelper.GenerateStringContent(ApiHelper.SerializeObj(customer));
            var response = await httpClient.PostAsync("api/Customer/DeleteCustomer", content);
        }

        public async Task<List<Customer>> GetAllCustomers()
        {
            return await httpClient.GetFromJsonAsync<List<Customer>>("api/Customer/GetAllCustomers");
        }

        public async Task UpdateCustomer(Customer customer)
        {
            var content = ApiHelper.GenerateStringContent(ApiHelper.SerializeObj(customer));
            var response = await httpClient.PostAsync("api/Customer/UpdateCustomer", content);
        }
    }
}