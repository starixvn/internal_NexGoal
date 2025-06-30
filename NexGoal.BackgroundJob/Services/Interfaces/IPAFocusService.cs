namespace NexGoal.BackgroundJob.Services
{
    using System.Threading.Tasks;
    using NexGoal.Core.Models;

    public interface IPAFocusService
    {
        Task<CommandResult> GenerateAsync();

        Task SendMailAsync();

        Task SendNotificationAsync();

        Task SendSkypeMessage();
    }
}