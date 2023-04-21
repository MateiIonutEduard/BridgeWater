using System.Security.Cryptography;

namespace BridgeWater.Services
{
    public class CryptoService : ICryptoService
    {
        readonly IAppSettings appSettings;

        public CryptoService(IAppSettings appSettings)
        { this.appSettings = appSettings; }

        public string Encrypt(string data)
        {
            byte[] key = Convert.FromBase64String(appSettings.key);
            byte[] salt = Convert.FromBase64String(appSettings.salt);

            using (Aes cryptoModel = Aes.Create())
            {
                var crypt = cryptoModel.CreateEncryptor(key, salt);
                MemoryStream ms = new MemoryStream();

                using (CryptoStream cs = new CryptoStream(ms, crypt, CryptoStreamMode.Write))
                {
                    using (StreamWriter sw = new StreamWriter(cs))
                        sw.Write(data);

                    byte[] buffer = ms.ToArray();
                    string encrypted = Convert.ToBase64String(buffer);
                    return encrypted;
                }
            }
        }

        public string Decrypt(string data)
        {
            byte[] key = Convert.FromBase64String(appSettings.key);
            byte[] salt = Convert.FromBase64String(appSettings.salt);
            byte[] cypherText = Convert.FromBase64String(data);

            using (Aes cryptoModel = Aes.Create())
            {
                var crypt = cryptoModel.CreateDecryptor(key, salt);
                MemoryStream ms = new MemoryStream(cypherText);

                using (CryptoStream cs = new CryptoStream(ms, crypt, CryptoStreamMode.Read))
                {
                    string plain = string.Empty;

                    using (StreamReader sr = new StreamReader(cs))
                        plain = sr.ReadToEnd();

                    return plain;
                }
            }
        }
    }
}
