using Microsoft.Extensions.Logging;

namespace SharedLib.Options
{
    public record OpenAiOptions
    {
        public required string Endpoint { get; init; }

        public required string ApiKey { get; init; }

        public required string EmbeddingsDeploymentName { get; init; }

        public required string CompletionsDeploymentName { get; init; }

        public required string MaxConversationTokens { get; init; }

        public required string MaxCompletionTokens { get; init; }

        public required string MaxEmbeddingTokens { get; init; }

        /// <remarks>When using Azure OpenAI, this should be the "Deployment name" of the chat model.</remarks>
        public string ChatModel { get; set; } = "gpt-3.5-turbo-16k";

        public required ILogger Logger { get; init; }
    }
}