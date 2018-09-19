namespace NexGoal.AutoService.Services
{
    using NexGoal.AutoService.Models;

    public interface IPeriodService
    {
        CommandResult InsertEmployeeSettings();

        CommandResult GeneratePA();

        CommandResult EditStatus();
    }
}