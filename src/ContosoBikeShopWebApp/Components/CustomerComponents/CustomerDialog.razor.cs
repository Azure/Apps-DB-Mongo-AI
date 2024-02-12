using ContosoBikeShopWebApp.Services;
using Microsoft.AspNetCore.Components;
using SharedLib.Models;

namespace ContosoBikeShopWebApp.Components.CustomerComponents
{
    public partial class CustomerDialog : ComponentBase
    {
        [Parameter] public Customer Customer { get; set; } = new();
        [Parameter] public EventCallback<bool> OnClose { get; set; }
        [Parameter] public string? customerId { get; set; }

        [Inject] public required CustomerClientService customerService { get; set; }

        public bool ShowDialog { get; set; }

        [Parameter] public EventCallback<bool> CloseEventCallback { get; set; }

        public void Show()
        {
            ResetDialog();
            ShowDialog = true;
            StateHasChanged();
        }

        public void Close()
        {
            ShowDialog = false;
            StateHasChanged();
        }

        private void ResetDialog()
        {
            Customer = new Customer();
        }

        private Task Cancel()
        {
            return OnClose.InvokeAsync(false);
        }

        private Task Ok()
        {
            return OnClose.InvokeAsync(true);
        }

        protected async Task<Task> HandleValidSubmit()
        {
            //if (Customer.id == null)
            //{
            //    Customer.id = Guid.NewGuid().ToString();
            //    await customerService.AddCustomer(Customer);
            //    return OnClose.InvokeAsync(true);
            //}
            await customerService.AddCustomer(Customer);

            await customerService.UpdateCustomer(Customer);
            return OnClose.InvokeAsync(true);
        }
    }
}