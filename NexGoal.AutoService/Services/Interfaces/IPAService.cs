namespace NexGoal.AutoService.Services
{
    using NexGoal.Core.Models;

    public interface IPAService
    {
        CommandResult EditGrade();

        CommandResult EditWhenEvaluationFinished();

        CommandResult EditStatus();
    }
}