using System.Security.Claims;
using ContosoBikeShopWebApp.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace ContosoBikeShopWebApp.Components.Chatbot
{
    public class ChatState
    {
        private readonly OpenAIPromptExecutionSettings _aiSettings = new()
            { ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions };

        private readonly Kernel _kernel;
        private readonly ILogger _logger;
        private readonly NavigationManager _navigationManager;
        private readonly ClaimsPrincipal _user;
        private readonly ChatClientService _chatClientService;
        private HttpClient _httpClient;
        private string _sessionId = string.Empty;
        private readonly string _systemPromptRetailAssistant;

        public ChatState(NavigationManager nav, Kernel kernel, ILoggerFactory loggerFactory,
            ChatClientService chatClientService)
        {
            _navigationManager = nav;
            _chatClientService = chatClientService;
            _logger = loggerFactory.CreateLogger(typeof(ChatState));

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                var completionService = kernel.GetRequiredService<IChatCompletionService>();
                _logger.LogDebug("ChatName: {model}", completionService.Attributes["DeploymentName"]);
            }

            _kernel = kernel;

            _systemPromptRetailAssistant = @"
        You are an intelligent assistant for the Cosmic Works Bike Company. 
        You are designed to provide helpful answers to user questions about 
        product, product category, customer and sales order information provided in JSON format below.

        Instructions:
        - Only answer questions related to the information provided below,
        - Don't reference any product, customer, or salesOrder data not provided below.
        - If you're unsure of an answer, you can say ""I don't know"" or ""I'm not sure"" and recommend users search themselves.

        Text of relevant information:";

            Messages = new ChatHistory(_systemPromptRetailAssistant);

            Messages.AddAssistantMessage("Hi! I'm the Cosmic Works Bike Concierge. How can I help?");
        }

        public ChatHistory Messages { get; }

        public async Task AddUserMessageAsync(string userText, Action onMessageAdded)
        {
            if (string.IsNullOrEmpty(_sessionId))
            {
                _sessionId = await _chatClientService.CreateChatSession();
            }

            // Store the user's message
            Messages.AddUserMessage(userText);
            onMessageAdded();

            // Get and store the AI's response message
            try
            {
                //TODO: Update to utilize Semantic Kernel Plugin for "Own Data" when available for c# 
                var vectorResponse = await _chatClientService.GetChatCompletion(_sessionId, userText);

                if (!string.IsNullOrWhiteSpace(vectorResponse))
                {
                    Messages.Add(new ChatMessageContent(AuthorRole.Assistant, vectorResponse));
                }
            }
            catch (Exception e)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError(e, "Error getting chat completions.");
                }

                Messages.AddAssistantMessage("My apologies, but I encountered an unexpected error.");
            }

            onMessageAdded();
        }
    }
}