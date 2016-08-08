namespace NexGoal.Core.Services
{
    public interface IBaseMailService
    {
        void SendEmail(MailModel email);

        string ReadTextFile(string filePath);
    }
}