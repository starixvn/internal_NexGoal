namespace NexGoal.AutoService.Services
{
    using NexGoal.Core.Models;

    public interface IPeriodService
    {
        CommandResult CreatePeriod();

        CommandResult CreateEmployeeSettings();

        CommandResult GeneratePA();
    }
}