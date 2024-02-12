using Microsoft.AspNetCore.Mvc;
using SharedLib.Models;
using SharedLib.Services;

namespace ContosoBikeShopServer.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CustomerController(CustomerService customerService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> AddCustomer([FromBodyAttribute]Customer customer)
        {
            return Ok(await customerService.AddCustomer(customer));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCustomer(Customer customer)
        {
            await customerService.DeleteCustomer(customer);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCustomers()
        {
            var customers = await customerService.GetAllCustomers();
            return Ok(customers);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCustomer(Customer customer)
        {
            return Ok(await customerService.UpdateCustomer(customer));
        }
    }
}