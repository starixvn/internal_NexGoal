namespace NexGoal.AutoService.Services
{
    using System.Threading.Tasks;
    using NexGoal.Core.Models;

    public interface IPAService
    {
        Task<CommandResult> EditGradeAsync();

        Task<CommandResult> EditWhenEvaluationFinishedAsync();

        Task<CommandResult> EditStatusAsync();

        Task SendMailAsync();

        Task SendNotificationAsync();

        Task SendSkypeMessage();
    }
}