using ContosoBikeShopWebApp.Components;
using ContosoBikeShopWebApp.Extensions;
using ContosoBikeShopWebApp.Services;

namespace ContosoBikeShopWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddScoped<ProductClientService, ProductClientService>();
            builder.Services.AddScoped<CustomerClientService, CustomerClientService>();
            builder.Services.AddScoped<ChatClientService, ChatClientService>();
            builder.AddAIServices();
            builder.Services.AddHttpClient("WebApiClient",
                c => c.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ApiBaseUrl")));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
