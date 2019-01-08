namespace NexGoal.AutoService.Services
{
    using NexGoal.Core.Models;

    public interface INominationService
    {
        void SendMail();

        void SendNotification();

        CommandResult EditWhenFinished();
    }
}