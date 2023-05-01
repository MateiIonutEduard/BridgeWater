namespace BridgeWater.Services
{
    public interface ICryptoService
    {
        string Encrypt(string data);
        string ComputeHash(byte[] buffer);
        string Decrypt(string data);
    }
}
