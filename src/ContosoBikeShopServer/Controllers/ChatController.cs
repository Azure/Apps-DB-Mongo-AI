using Microsoft.AspNetCore.Mvc;
using SharedLib.Services;

namespace ContosoBikeShopServer.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class ChatController(ChatService chatService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> GetChatCompletion([FromBody] ChatCompletionContent content)
        {
            var response =
                await chatService.GetChatCompletionAsync(content.sessionId, content.userPrompt, content.collectionName);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> CreateChatSession()
        {
            var session = await chatService.CreateNewChatSession();
            return Ok(session.Id);
        }

        public record ChatCompletionContent(string sessionId, string userPrompt, string collectionName);
    }
}