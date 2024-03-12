namespace NexGoal.AutoService.Services
{
    using System.Threading.Tasks;
    using NexGoal.Core.Models;

    public interface IRedmineDateService
    {
        Task<CommandResult> CreatePeriodAsync();

        Task<CommandResult> CreateEmployeeSettingsAsync();

        Task<CommandResult> GeneratePAAsync();
    }
}