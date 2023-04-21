#pragma warning disable

namespace BridgeWater.Services
{
    public class AdminSettings : IAdminSettings
    {
        public string host { get; set; }
        public int? port { get; set; }
        public string client { get; set; }
        public string secret { get; set; }
    }
}
