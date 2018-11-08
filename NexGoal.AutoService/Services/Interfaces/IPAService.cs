namespace NexGoal.AutoService.Services
{
    using NexGoal.AutoService.Models;

    public interface IPAService
    {
        CommandResult EditWhenEvaluationFinished();

        CommandResult EditStatus();
    }
}