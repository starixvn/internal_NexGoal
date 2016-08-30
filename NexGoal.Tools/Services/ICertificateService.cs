namespace NexGoal.Tools
{
    using System.Security.Cryptography.X509Certificates;

    public interface ICertificateService
    {
        int GetMaxDataLength(int keySize);

        bool IsKeySizeValid(int keySize);

        string Encrypt(X509Certificate2 cert2, string plainText);

        string Decrypt(X509Certificate2 cert2, string cipherText);
    }
}