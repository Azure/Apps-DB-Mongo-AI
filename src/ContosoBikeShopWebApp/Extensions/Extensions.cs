using Microsoft.SemanticKernel;
using SharedLib.Options;

namespace ContosoBikeShopWebApp.Extensions
{
    public static class Extensions
    {
        public static void AddAIServices(this WebApplicationBuilder builder)
        {
            var openAIOptions = builder.Configuration.GetSection("OpenAiOptions").Get<OpenAiOptions>();

            if (!string.IsNullOrWhiteSpace(openAIOptions?.ApiKey))
            {
                var kernelBuilder = builder.Services.AddKernel();


                if (!string.IsNullOrWhiteSpace(openAIOptions.Endpoint))
                {
                    kernelBuilder.AddAzureOpenAIChatCompletion(openAIOptions.CompletionsDeploymentName,
                        openAIOptions.Endpoint, openAIOptions.ApiKey, null, null, new HttpClient());
                }
                else
                {
                    kernelBuilder.AddOpenAIChatCompletion(openAIOptions.CompletionsDeploymentName,
                        openAIOptions.ApiKey);
                }
            }
        }
    }
}