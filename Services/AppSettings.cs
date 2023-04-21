#pragma warning disable

namespace BridgeWater.Services
{
    public class AppSettings : IAppSettings
    {
        public string key { get; set; }
        public string salt { get; set; }
    }
}
