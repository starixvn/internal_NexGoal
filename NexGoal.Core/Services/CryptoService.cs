namespace NexGoal.Core.Services
{
    using System;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using NexGoal.Core.Models;

    public class CryptoService : ICryptoService
    {
        private const bool OptimalAsymmetricEncryptionPadding = false;

        private readonly AppSettings appSettings;
        private readonly RSACryptoServiceProvider privateRSA;
        private readonly RSACryptoServiceProvider publicRSA;

        public CryptoService(
            AppSettings appSettings,
            X509Certificate2 cert2 = null)
        {
            this.appSettings = appSettings;
            var cert = cert2 ?? InitCertificate();
            privateRSA = GetPrivateKeyRsaCryptoServiceProvider(cert);
            publicRSA = GetPublicKeyRsaCryptoServiceProvider(cert);
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

        private X509Certificate2 InitCertificate()
        {
            ////var cert = X509Certificate2.CreateFromEncryptedPemFile(configuration.CurrentValue.CertFilePath, configuration.CurrentValue.PasswordToSignCertFile);
            const X509KeyStorageFlags flags = X509KeyStorageFlags.Exportable;
            return new X509Certificate2(appSettings.CertFilePath, appSettings.PasswordToSignCertFile, flags);
        }

        private static RSACryptoServiceProvider GetPublicKeyRsaCryptoServiceProvider(X509Certificate2 certificate)
        {
            RSA rsa = certificate.GetRSAPublicKey();
            RSAParameters rsaParameters = rsa.ExportParameters(false);

            var rsacsp = new RSACryptoServiceProvider(rsa.KeySize);
            rsacsp.ImportParameters(rsaParameters);

            return rsacsp;
        }

        private static RSACryptoServiceProvider GetPrivateKeyRsaCryptoServiceProvider(X509Certificate2 certificate)
        {
            RSA rsa = certificate.GetRSAPrivateKey();
            RSAParameters rsaParameters = rsa.ExportParameters(true);

            var rsacsp = new RSACryptoServiceProvider(rsa.KeySize);
            rsacsp.ImportParameters(rsaParameters);

            return rsacsp;
        }
    }
}