namespace NexGoal.AutoService.Services
{
    using System.Threading.Tasks;
    using NexGoal.Core.Models;

    public interface INominationService
    {
        Task SendMailAsync();

        Task SendNotificationAsync();

        Task<CommandResult> EditWhenFinishedAsync();

        Task SendSkypeMessageAsync();
    }
}