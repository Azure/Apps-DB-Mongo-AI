namespace ContosoBikeShopWebApp.Services
{
    public class ChatClientService(IHttpClientFactory clientFactory)
    {
        private readonly HttpClient httpClient = clientFactory.CreateClient("WebApiClient");

        public async Task<string?> CreateChatSession()
        {
            var response = await httpClient.GetAsync("api/Chat/CreateChatSession");
            return await ApiHelper.ReadContent(response);
        }

        public async Task<string> GetChatCompletion(string sessionId, string userPrompt,
            string collectionName = "products")
        {
            var content =
                ApiHelper.GenerateStringContent(
                    ApiHelper.SerializeObj(new ChatCompletionContent(sessionId, userPrompt, collectionName)));

            var response = await httpClient.PostAsync("api/Chat/GetChatCompletion", content);

            var result = await ApiHelper.ReadContent(response);
            return result;
        }

        private record ChatCompletionContent(string sessionId, string userPrompt, string collectionName);
    }
}