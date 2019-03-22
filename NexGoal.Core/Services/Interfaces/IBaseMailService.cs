namespace NexGoal.Core.Services
{
    using System;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;

    public interface IBaseMailService
    {
        Task SendMailAsync(MailModel email);

        string ReadTextFile(string filePath);

        string FormatDate(DateTime date);
    }
}