namespace NexGoal.AutoService.Services
{
    using NexGoal.AutoService.Models;

    public interface IPAService
    {
        CommandResult EditGrade();

        CommandResult EditWhenEvaluationFinished();

        CommandResult EditStatus();
    }
}