using ContosoBikeShopWebApp.Services;
using Microsoft.AspNetCore.Components;
using SharedLib.Models;

namespace ContosoBikeShopWebApp.Components.ProductComponents
{
    public partial class ProductDialog : ComponentBase
    {
        [Parameter] public Product? Product { get; set; } = new();
        [Parameter] public EventCallback<bool> OnClose { get; set; }
        [Parameter] public string? productId { get; set; }

        [Inject] public ProductClientService productService { get; set; }

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
            Product = new Product();
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
            if (Product.id == null)
            {
                await productService.AddProduct(Product);
                return OnClose.InvokeAsync(true);
            }

            await productService.UpdateProduct(Product);
            return OnClose.InvokeAsync(true);
        }
    }
}