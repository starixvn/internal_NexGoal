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

        Task<CommandResult> EditSelfEvaluationAsync(EditSelfEvaluationModel model);

        Task<CommandResult> EditEvaluatorEvaluationAsync(EditEvaluatorEvaluationModel model);

        Task<CommandResult> EditReviewerEvaluationAsync(EditReviewerEvaluationModel model);

        Task<IEnumerable<ProbationerModel>> GetProbationersAsync();

        Task<HRMModel> GetHRM(int employeeId);

        Task<CommandResult> ConfirmEvaluatorResultAsync(int employeeId, int actorId);

        Task<IEnumerable<BaseProbationerModel>> GetProbationersNotInEvaluationAsync();

        Task<CommandResult> AddNewEmployeesAsync(IEnumerable<int> employeeIds);
    }
}