#pragma warning disable

namespace BridgeWater.Services
{
    public interface IAppSettings
    {
        string key { get; set; }
        string salt { get; set; }
    }
}
