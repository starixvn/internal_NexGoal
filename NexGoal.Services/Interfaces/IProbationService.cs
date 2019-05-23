namespace NexGoal.Services
{
    using NexGoal.Core.Models;
    using NexGoal.Models.Probation;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IProbationService
    {
        Task<CommandResult> EditSettingAsync(IEnumerable<ProbationSettingModel> settings);

        Task<IEnumerable<ProbationEvaluationModel>> GetProbationersAsync(int actorId);

        Task<CommandResult> EditSelfEvaluationAsync(int employeeId, IEnumerable<EditSelfEvaluationModel> selfEvaluations);

        Task<CommandResult> EditEvaluatorEvaluationAsync(int employeeId, IEnumerable<EditSelfEvaluationModel> evaluatorEvaluations);

        Task<CommandResult> EditReviewerEvaluationAsync(int employeeId, int finalScore);
    }
}