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

            publicRSA = new RSACryptoServiceProvider(KeySize);
            publicRSA.FromXmlString(ReadTextFile(appSettings.RSAPublicKeyPath));

            privateRSA = new RSACryptoServiceProvider(KeySize);
            privateRSA.FromXmlString(ReadTextFile(appSettings.RSAPrivateKeyPath));
        }

        public string Encrypt(string plainText)
        {
            var cipherText = string.Empty;

            if (!string.IsNullOrWhiteSpace(plainText))
            {
                if (appSettings.EnableEncrypt)
                {
                    byte[] bytesToEncrypt = ASCIIEncoding.ASCII.GetBytes(plainText);

                    byte[] encryptedBytes = publicRSA.Encrypt(bytesToEncrypt, OptimalAsymmetricEncryptionPadding);
                    cipherText = Convert.ToBase64String(encryptedBytes);
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
                    byte[] plainbytes = privateRSA.Decrypt(bytesToDecrypt, OptimalAsymmetricEncryptionPadding);
                    var enc = new ASCIIEncoding();
                    plainText = enc.GetString(plainbytes);
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                publicRSA.Dispose();
                privateRSA.Dispose();
            }
        }
    }
}