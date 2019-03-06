namespace NexGoal.AutoService.Services
{
    using System.Threading.Tasks;
    using NexGoal.Core.Models;

    public interface IPAService
    {
        CommandResult EditGrade();

        CommandResult EditWhenEvaluationFinished();

        CommandResult EditStatus();

        void SendMail();

        void SendNotification();

        Task SendSkypeMessage()
    }
}