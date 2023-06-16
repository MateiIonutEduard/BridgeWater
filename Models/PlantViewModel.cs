namespace BridgeWater.Models
{
    public class PlantViewModel
    {
        public int? id { get; set; }
        public string? username { get; set; }
        public string? body { get; set; }
        public int? rating { get; set; }
        public int? depth { get; set; }
        public DateTime? createdAt { get; set; }
        public int accountId { get; set; }
    }
}
