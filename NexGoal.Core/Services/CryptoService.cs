namespace NexGoal.Core.Services
{
    using System;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using Microsoft.Extensions.Options;
    using NexGoal.Core.Models;

    public class CryptoService : ICryptoService
    {
        private const bool OptimalAsymmetricEncryptionPadding = false;

        private readonly X509Certificate2 cert2;
        private readonly IOptionsMonitor<AppSettings> configuration;

        public CryptoService(
            IOptionsMonitor<AppSettings> configuration,
            X509Certificate2 cert2 = null)
        {
            this.configuration = configuration;
            this.cert2 = cert2 ?? InitCertificate();
        }

        public string Encrypt(string plainText)
        {
            var cipherText = string.Empty;

            if (!string.IsNullOrWhiteSpace(plainText))
            {
                if (configuration.CurrentValue.EnableEncrypt)
                {
                    byte[] bytesToEncrypt = ASCIIEncoding.ASCII.GetBytes(plainText);

                    RSACryptoServiceProvider rsa = GetPublicKeyRsaCryptoServiceProvider(cert2);
                    byte[] encryptedBytes = rsa.Encrypt(bytesToEncrypt, OptimalAsymmetricEncryptionPadding);
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
                if (configuration.CurrentValue.EnableEncrypt)
                {
                    RSACryptoServiceProvider rsa = GetPrivateKeyRsaCryptoServiceProvider(cert2);
                    byte[] bytesToDecrypt = Convert.FromBase64String(cipherText);
                    byte[] plainbytes = rsa.Decrypt(bytesToDecrypt, OptimalAsymmetricEncryptionPadding);
                    ASCIIEncoding enc = new ASCIIEncoding();
                    plainText = enc.GetString(plainbytes);
                }
                else
                {
                    plainText = cipherText;
                }
            }

            return plainText;
        }

        private X509Certificate2 InitCertificate()
        {
            var flags = X509KeyStorageFlags.Exportable;

            return new X509Certificate2(configuration.CurrentValue.CertFilePath, configuration.CurrentValue.PasswordToSignCertFile, flags);
        }

        private RSACryptoServiceProvider GetPublicKeyRsaCryptoServiceProvider(X509Certificate2 certificate)
        {
            RSA rsa = certificate.GetRSAPublicKey();
            RSAParameters rsaParameters = rsa.ExportParameters(false);

            var rsacsp = new RSACryptoServiceProvider(rsa.KeySize, new CspParameters
            {
                ProviderType = 24,
                ProviderName = "Microsoft Enhanced RSA and AES Cryptographic Provider",
            });
            rsacsp.ImportParameters(rsaParameters);

            return rsacsp;
        }

        private RSACryptoServiceProvider GetPrivateKeyRsaCryptoServiceProvider(X509Certificate2 certificate)
        {
            RSA rsa = certificate.GetRSAPrivateKey();
            RSAParameters rsaParameters = rsa.ExportParameters(true);

            var rsacsp = new RSACryptoServiceProvider(rsa.KeySize, new CspParameters
            {
                ProviderType = 24,
                ProviderName = "Microsoft Enhanced RSA and AES Cryptographic Provider",
            });
            rsacsp.ImportParameters(rsaParameters);

            return rsacsp;
        }
    }
}