using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SharedLib.Models
{
    public record Session
    {
        public Session()
        {
            Id = Guid.NewGuid().ToString();
            Type = nameof(Session);
            SessionId = Id;
            TokensUsed = 0;
            Name = "New Chat";
            Messages = new List<Message>();
        }

        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; }

        public string Type { get; set; }

        public string SessionId { get; set; }

        public int? TokensUsed { get; set; }

        public string Name { get; set; }

        [BsonIgnore] public List<Message> Messages { get; set; }

        public void AddMessage(Message message)
        {
            Messages.Add(message);
        }

        public void UpdateMessage(Message message)
        {
            var match = Messages.Single(m => m.Id == message.Id);
            var index = Messages.IndexOf(match);
            Messages[index] = message;
        }
    }
}