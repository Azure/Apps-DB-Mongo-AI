using SharedLib.Interface;
using SharedLib.Models;

namespace SharedLib.Services
{
    public class CustomerService : ICustomerService
    {
        public MongoDbService _mongoDbService;

        public CustomerService(MongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }

        public async Task<Customer> AddCustomer(Customer customer)
        {
            return await _mongoDbService.UpsertCustomerAsync(customer);
        }

        public async Task<List<Customer>> GetAllCustomers()
        {
            return await _mongoDbService.GetCustomersAsync();
        }

        public async Task<Customer> UpdateCustomer(Customer customer)
        {
            return await _mongoDbService.UpsertCustomerAsync(customer);
        }

        public async Task DeleteCustomer(Customer customer)
        {
            await _mongoDbService.DeleteCustomerAsync(customer);
        }
    }
}