namespace NexGoal.Services
{
    using NexGoal.Core.Models;
    using NexGoal.Models.Probation;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IProbationService
    {
        Task<CommandResult> EditSetting(IEnumerable<ProbationSettingModel> settings);

        Task<IEnumerable<ProbationEvaluationModel>> GetProbationersAsync(int actorId);

        Task<CommandResult> EditSelfEvaluation(int employeeId, IEnumerable<EditSelfEvaluationModel> selfEvaluations);
    }
}