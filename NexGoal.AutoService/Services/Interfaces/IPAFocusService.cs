namespace NexGoal.AutoService.Services
{
    using System.Threading.Tasks;
    using NexGoal.Core.Models;

    public interface IPAFocusService
    {
        CommandResult Generate();

        void SendMail();

        void SendNotification();

        Task SendSkypeMessage();
    }
}