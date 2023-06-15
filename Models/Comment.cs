using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace BridgeWater.Models
{
    public class Comment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string? body { get; set; }
        public int? rating { get; set; }
        public DateTime createdAt { get; set; }

        public bool? isDeleted { get; set; }
        public string? replyTo { get; set; }
        public int accountId { get; set; }
    }
}
