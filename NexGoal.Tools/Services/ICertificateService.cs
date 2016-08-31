namespace NexGoal.Tools
{
    using System.Security.Cryptography.X509Certificates;

    public interface ICertificateService
    {
        string Encrypt(X509Certificate2 cert2, string plainText);

        string Decrypt(X509Certificate2 cert2, string cipherText);
    }
}