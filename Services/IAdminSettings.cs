namespace BridgeWater.Services
{
    public interface IAdminSettings
    {
        string host { get; set; }
        int? port { get; set; }
        string client { get; set; }
        string secret { get; set; }
    }
}
