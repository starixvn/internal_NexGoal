namespace NexGoal.Core.Services
{
    using System;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;

    public class CertificateService : ICertificateService
    {
        private const bool OptimalAsymmetricEncryptionPadding = false;

        public string Encrypt(string plainText)
        {
            string cipherText = string.Empty;

            if (!string.IsNullOrWhiteSpace(plainText))
            {
                if (AppSetting.EnableEncrypt)
                {
                    X509Certificate2 cert2 = GetCertificate();
                    RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)cert2.PublicKey.Key;
                    byte[] bytesToEncrypt = ASCIIEncoding.ASCII.GetBytes(plainText);
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
            string plainText = string.Empty;

            if (!string.IsNullOrWhiteSpace(cipherText))
            {
                if (AppSetting.EnableEncrypt)
                {
                    X509Certificate2 cert2 = GetCertificate();
                    RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)cert2.PrivateKey;
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

        private X509Certificate2 GetCertificate()
        {
            X509Certificate2 cert2;
            if (!string.IsNullOrWhiteSpace(AppSetting.PasswordToSignTheCertificateFile))
            {
                cert2 = new X509Certificate2(AppSetting.CertificateFilePath, AppSetting.PasswordToSignTheCertificateFile);
            }
            else
            {
                cert2 = new X509Certificate2(AppSetting.CertificateFilePath);
            }

            return cert2;
        }
    }
}