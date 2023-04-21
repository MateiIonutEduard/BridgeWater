namespace BridgeWater.Services
{
    public interface IAdminService
    {
        int SendEmail(string to, string subject, string body);
    }
}
