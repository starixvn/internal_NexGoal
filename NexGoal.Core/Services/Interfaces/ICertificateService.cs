namespace NexGoal.Core.Services
{
    public interface ICertificateService
    {
        string Encrypt(string plainText);

        string Decrypt(string cipherText);
    }
}