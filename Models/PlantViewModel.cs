namespace BridgeWater.Models
{
    public class PlantViewModel
    {
        public string? Id { get; set; }
        public double? Stars { get; set; }

        public string? name { get; set; }
        public string? imageUrl { get; set; }

        public Description? description { get; set; }
        public CommentViewModel[]? comments { get; set; }

        public string? category { get; set; }
    }
}
