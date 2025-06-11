namespace NexGoal.Core.Services
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using NexGoal.Core.Models;

    public class CryptoService : ICryptoService, IDisposable
    {
        private const bool OptimalAsymmetricEncryptionPadding = false;
        private readonly RSACryptoServiceProvider publicRSA;
        private readonly RSACryptoServiceProvider privateRSA;
        private const short KeySize = 2048;
        private readonly AppSettings appSettings;

        public CryptoService(AppSettings appSettings)
        {
            this.appSettings = appSettings;

            if (appSettings.EnableEncrypt)
            {
                publicRSA = new RSACryptoServiceProvider(KeySize);
                privateRSA = new RSACryptoServiceProvider(KeySize);

                try
                {
                    var publicKey = ReadTextFile(appSettings.RSAPublicKeyPath);
                    var privateKey = ReadTextFile(appSettings.RSAPrivateKeyPath);

                    if (!string.IsNullOrEmpty(publicKey))
                        publicRSA.FromXmlString(publicKey);
                    if (!string.IsNullOrEmpty(privateKey))
                        privateRSA.FromXmlString(privateKey);
                }
                catch
                {
                    // If there's any error reading or parsing the keys, disable encryption
                    this.appSettings.EnableEncrypt = false;
                }
            }
        }

        public string Encrypt(string plainText)
        {
            if (!appSettings.EnableEncrypt || string.IsNullOrWhiteSpace(plainText))
                return plainText;

            try
            {
                byte[] bytesToEncrypt = ASCIIEncoding.ASCII.GetBytes(plainText);
                byte[] encryptedBytes = publicRSA.Encrypt(bytesToEncrypt, OptimalAsymmetricEncryptionPadding);
                return Convert.ToBase64String(encryptedBytes);
            }
            catch
            {
                return plainText;
            }
        }

        public string Decrypt(string cipherText)
        {
            if (!appSettings.EnableEncrypt || string.IsNullOrWhiteSpace(cipherText))
                return cipherText;

            try
            {
                byte[] bytesToDecrypt = Convert.FromBase64String(cipherText);
                byte[] plainbytes = privateRSA.Decrypt(bytesToDecrypt, OptimalAsymmetricEncryptionPadding);
                return ASCIIEncoding.ASCII.GetString(plainbytes);
            }
            catch
            {
                return cipherText;
            }
        }

        private static string ReadTextFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return string.Empty;

            return File.ReadAllText(filePath);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                publicRSA?.Dispose();
                privateRSA?.Dispose();
            }
        }
    }
}