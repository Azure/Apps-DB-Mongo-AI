using SharedLib.Models;

namespace SharedLib.Interface
{
    public interface ICustomerService
    {
        Task<Customer> AddCustomer(Customer customer);
        Task DeleteCustomer(Customer customer);
        Task<List<Customer>> GetAllCustomers();
        Task<Customer> UpdateCustomer(Customer customer);
    }
}