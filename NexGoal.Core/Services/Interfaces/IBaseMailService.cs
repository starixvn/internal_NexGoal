namespace NexGoal.Core.Services
{
    using Models;

    public interface IBaseMailService
    {
        void SendEmail(MailModel email);

        string ReadTextFile(string filePath);
    }
}