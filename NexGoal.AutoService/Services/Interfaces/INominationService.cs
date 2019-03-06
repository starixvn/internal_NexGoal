namespace NexGoal.AutoService.Services
{
    using System.Threading.Tasks;
    using NexGoal.Core.Models;

    public interface INominationService
    {
        void SendMail();

        void SendNotification();

        CommandResult EditWhenFinished();

        Task SendSkypeMessage()
    }
}