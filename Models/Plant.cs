using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BridgeWater.Models
{
    public class Plant
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string? name { get; set; }
        public string? imageUrl { get; set; }

        public Description? description { get; set; }
        public List<Comment>? comments { get; set; }

        public string? category { get; set; }
    }
}
