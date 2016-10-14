namespace NexGoal.Core.Services
{
    using System;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;

    public class CertificateService : ICertificateService
    {
        private const bool OptimalAsymmetricEncryptionPadding = false;
        private static X509Certificate2 cert2 = null;

        public CertificateService()
        {
            InitCertificate();
        }

        public CertificateService(X509Certificate2 cert)
        {
            cert2 = cert;
        }

        public string Encrypt(string plainText)
        {
            string cipherText = string.Empty;

            if (!string.IsNullOrWhiteSpace(plainText))
            {
                if (AppSetting.EnableEncrypt)
                {
                    byte[] bytesToEncrypt = ASCIIEncoding.ASCII.GetBytes(plainText);

                    int keySize = cert2.PublicKey.Key.KeySize,
                        maxLength = GetMaxDataLength(keySize);

                    if (bytesToEncrypt.Length > maxLength)
                    {
                        throw new Exception(string.Format("Maximum data length is {0}", maxLength));
                    }

                    if (!IsKeySizeValid(keySize))
                    {
                        throw new Exception("Key size is not valid");
                    }

                    if (string.IsNullOrEmpty(cert2.PublicKey.Key.ToString()))
                    {
                        throw new Exception("Key is null or empty");
                    }

                    RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)cert2.PublicKey.Key;
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
                    int keySize = cert2.PrivateKey.KeySize;
                    if (!IsKeySizeValid(keySize))
                    {
                        throw new Exception("Key size is not valid");
                    }

                    if (string.IsNullOrEmpty(cert2.PrivateKey.ToString()))
                    {
                        throw new Exception("Key is null or empty");
                    }

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

        public void InitCertificate()
        {
            if (cert2 == null)
            {
                X509KeyStorageFlags flags = X509KeyStorageFlags.MachineKeySet;
                cert2 = new X509Certificate2(AppSetting.CertificateFilePath, AppSetting.PasswordToSignTheCertificateFile, flags);
            }
        }

        private int GetMaxDataLength(int keySize)
        {
            if (OptimalAsymmetricEncryptionPadding)
            {
                return ((keySize - 384) / 8) + 7;
            }

            return ((keySize - 384) / 8) + 37;
        }

        private bool IsKeySizeValid(int keySize)
        {
            return keySize >= 384 && keySize <= 16384 && keySize % 8 == 0;
        }
    }
}