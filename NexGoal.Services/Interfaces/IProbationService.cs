namespace NexGoal.Services
{
    using NexGoal.Core.Models;
    using NexGoal.Models.Probation;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IProbationService
    {
        Task<CommandResult> EditSettingAsync(IEnumerable<EditProbationSettingModel> settings, int employeeId, int actorId);

        Task<IEnumerable<ProbationEvaluationModel>> GetProbationEvaluationsAsync(int actorId, int? employeeId);

        Task<IEnumerable<KPIGroupModel>> GetEvaluationDetailAsync(int employeeId);

        Task<CommandResult> EditSelfEvaluationAsync(int employeeId, string comment, IEnumerable<EditEvaluationModel> selfEvaluations);

        Task<CommandResult> EditEvaluatorEvaluationAsync(int employeeId, string comment, IEnumerable<EditEvaluationModel> evaluatorEvaluations);

        Task<CommandResult> EditReviewerEvaluationAsync(int employeeId, int finalScore, string comment);

        Task<IEnumerable<ProbationerModel>> GetProbationersAsync();
    }
}