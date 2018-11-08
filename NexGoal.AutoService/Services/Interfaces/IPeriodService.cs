namespace NexGoal.AutoService.Services
{
    using NexGoal.AutoService.Models;

    public interface IPeriodService
    {
        CommandResult CreatePeriod();

        CommandResult CreateEmployeeSettings();

        CommandResult GeneratePA();
    }
}