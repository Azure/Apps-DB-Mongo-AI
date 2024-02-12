using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharedLib.Interface;
using SharedLib.Models;
using SharedLib.Options;
using SharedLib.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.RegisterConfiguration();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.RegisterServices();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseWebAssemblyDebugging();
}

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();
app.MapFallbackToFile("index.html");
app.UseCors("AllowAll");

app.Run();

internal static class ProgramExtensions
{
    public static void RegisterConfiguration(this WebApplicationBuilder builder)
    {
        //builder.Configuration.AddUserSecrets<Program>(optional: true, reloadOnChange: true);
        builder.Configuration.AddJsonFile("appsettings.Development.json", true, true);

        builder.Services.AddOptions<OpenAiOptions>()
            .Bind(builder.Configuration.GetSection(nameof(OpenAiOptions)));


        builder.Services.AddOptions<MongoDb>()
            .Bind(builder.Configuration.GetSection(nameof(MongoDb)));

        builder.AddDbContext();
        builder.Services.AddScoped<IProductEf, ProductServiceEF>();
    }

    public static void AddDbContext(this WebApplicationBuilder builder)
    {
        var mongoDBSettings = builder.Configuration.GetSection("MongoDb").Get<MongoDb>();
        builder.Services.AddDbContext<RetailDbContext>(options =>
            options.UseMongoDB(mongoDBSettings.Connection ?? "", mongoDBSettings.DatabaseName ?? ""));
    }

    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddSingleton<ProductService, ProductService>();
        services.AddSingleton<CustomerService, CustomerService>();

        services.AddSingleton<OpenAiService, OpenAiService>(provider =>
        {
            var openAiOptions = provider.GetRequiredService<IOptions<OpenAiOptions>>();

            if (openAiOptions is null)
            {
                throw new ArgumentException(
                    $"{nameof(IOptions<OpenAiOptions>)} was not resolved through dependency injection.");
            }

            return new OpenAiService(
                openAiOptions.Value?.Endpoint ?? string.Empty,
                openAiOptions.Value?.ApiKey ?? string.Empty,
                openAiOptions.Value?.EmbeddingsDeploymentName ?? string.Empty,
                openAiOptions.Value?.CompletionsDeploymentName ?? string.Empty,
                maxConversationTokens: openAiOptions.Value?.MaxConversationTokens ?? string.Empty,
                maxCompletionTokens: openAiOptions.Value?.MaxCompletionTokens ?? string.Empty,
                maxEmbeddingTokens: openAiOptions.Value?.MaxEmbeddingTokens ?? string.Empty,
                logger: provider.GetRequiredService<ILogger<OpenAiService>>()
            );
        });

        services.AddSingleton<MongoDbService, MongoDbService>(provider =>
        {
            var mongoDbOptions = provider.GetRequiredService<IOptions<MongoDb>>();
            if (mongoDbOptions is null)
            {
                throw new ArgumentException(
                    $"{nameof(IOptions<MongoDb>)} was not resolved through dependency injection.");
            }

            return new MongoDbService(
                mongoDbOptions.Value?.Connection ?? string.Empty,
                mongoDbOptions.Value?.DatabaseName ?? string.Empty,
                mongoDbOptions.Value?.CollectionNames ?? string.Empty,
                mongoDbOptions.Value?.MaxVectorSearchResults ?? string.Empty,
                mongoDbOptions.Value?.VectorIndexType ?? string.Empty,
                provider.GetRequiredService<OpenAiService>(),
                provider.GetRequiredService<ILogger<MongoDbService>>()
            );
        });
        services.AddSingleton<ChatService, ChatService>(provider =>
        {
            var chatOptions = provider.GetRequiredService<IOptions<Chat>>();
            if (chatOptions is null)
            {
                throw new ArgumentException($"{nameof(IOptions<Chat>)} was not resolved through dependency injection");
            }

            return new ChatService(
                mongoDbService: provider.GetRequiredService<MongoDbService>(),
                openAiService: provider.GetRequiredService<OpenAiService>(),
                logger: provider.GetRequiredService<ILogger<ChatService>>()
            );
        });
    }
}