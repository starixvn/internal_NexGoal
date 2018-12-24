namespace NexGoal.AutoService.Services
{
    using NexGoal.Core.Models;

    public interface INominationService
    {
        CommandResult EditWhenFinished();

        void SendMail();
    }
}