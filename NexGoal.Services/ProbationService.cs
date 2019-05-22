namespace NexGoal.Services
{
    using NexGoal.Core.Extensions;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Probation;
    using NexGoal.Models.Probation;
    using NexGoal.Repositories;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class ProbationService : IProbationService
    {
        private readonly IProbationRepository probationRepository;

        private readonly IBaseService baseService;

        public ProbationService(IProbationRepository probationRepository, IBaseService baseService)
        {
            this.probationRepository = probationRepository;
            this.baseService = baseService;
        }

        public async Task<IEnumerable<ProbationEvaluationModel>> GetProbationersAsync(int actorId)
        {
            return await probationRepository.GetProbationersAsync(new GetProbationersCriteria
            {
                ActorId = actorId
            }).ConfigureAwait(false);
        }

        public async Task<CommandResult> EditSetting(IEnumerable<ProbationSettingModel> settings)
        {
            return await baseService.ExecuteNonQueryAsync(new UpdateSettingCriteria
            {
                Settings = settings.ToXML().ToString()
            }).ConfigureAwait(false);
        }

        public async Task<CommandResult> EditSelfEvaluation(int employeeId, IEnumerable<EditSelfEvaluationModel> selfEvaluations)
        {
            return await baseService.ExecuteNonQueryAsync(new UpdateSelfEvaluationCriteria
            {
                EmployeeId = employeeId,
                Details = selfEvaluations.ToXML().ToString()
            }).ConfigureAwait(false);
        }
    }
}