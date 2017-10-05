namespace NexGoal.Core.Services
{
    using System;

    public interface IBaseMailService
    {
        void SendEmail(MailModel email);

        string ReadTextFile(string filePath);

        string ChangeStartDate(DateTime startDate);

        string ChangeEndDate(DateTime endDate);
    }
}