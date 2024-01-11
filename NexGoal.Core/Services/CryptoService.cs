namespace NexGoal.Core.Services
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using NexGoal.Core.Models;

    public class CryptoService : ICryptoService
    {
        private const bool OptimalAsymmetricEncryptionPadding = false;
        private readonly string publicKey;
        private readonly string privateKey;
        private const short KeySize = 2048;
        private readonly AppSettings appSettings;

        public CryptoService(AppSettings appSettings)
        {
            this.appSettings = appSettings;
            publicKey = ReadTextFile(appSettings.RSAPublicKeyPath);
            privateKey = ReadTextFile(appSettings.RSAPrivateKeyPath);
        }

        public string Encrypt(string plainText)
        {
            var cipherText = string.Empty;

            if (!string.IsNullOrWhiteSpace(plainText))
            {
                if (appSettings.EnableEncrypt)
                {
                    byte[] bytesToEncrypt = ASCIIEncoding.ASCII.GetBytes(plainText);

                    using (var rsa = new RSACryptoServiceProvider(KeySize))
                    {
                        rsa.FromXmlString(publicKey);
                        byte[] encryptedBytes = rsa.Encrypt(bytesToEncrypt, OptimalAsymmetricEncryptionPadding);
                        cipherText = Convert.ToBase64String(encryptedBytes);
                    }
                }
                else
                {
                    cipherText = plainText;
                }
            } 

            return cipherText;
        }

        public string Decrypt(string cipherText)
        {
            var plainText = string.Empty;

            if (!string.IsNullOrWhiteSpace(cipherText))
            {
                if (appSettings.EnableEncrypt)
                {
                    byte[] bytesToDecrypt = Convert.FromBase64String(cipherText);
                    using (var rsa = new RSACryptoServiceProvider(KeySize))
                    {
                        rsa.FromXmlString(privateKey);
                        byte[] plainbytes = rsa.Decrypt(bytesToDecrypt, OptimalAsymmetricEncryptionPadding);
                        var enc = new ASCIIEncoding();
                        plainText = enc.GetString(plainbytes);
                    }
                }
                else
                {
                    plainText = cipherText;
                }
            }

            return plainText;
        }

        private static string ReadTextFile(string filePath)
        {
            var text = string.Empty;
            if(File.Exists(filePath))
            {
                text = File.ReadAllText(filePath);
            }

            return text;
        }
    }
}