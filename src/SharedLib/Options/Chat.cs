using Microsoft.Extensions.Logging;
using SharedLib.Services;

namespace SharedLib.Options
{
    public record Chat
    {
        public required MongoDbService MongoDbService { get; set; }

        public required OpenAiService OpenAiService { get; set; }

        public required ILogger Logger { get; init; }
    }
}