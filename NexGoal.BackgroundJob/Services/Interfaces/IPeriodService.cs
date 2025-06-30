namespace NexGoal.BackgroundJob.Services
{
    using System.Threading.Tasks;
    using NexGoal.Core.Models;

    public interface IPeriodService
    {
        Task<CommandResult> CreatePeriodAsync();

        Task<CommandResult> CreateEmployeeSettingsAsync();

        Task<CommandResult> GeneratePAAsync();
    }
}