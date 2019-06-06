namespace NexGoal.Services
{
    using NexGoal.Core.Extensions;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Probation;
    using NexGoal.Models.Probation;
    using NexGoal.Repositories;
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

        public async Task<IEnumerable<ProbationEvaluationModel>> GetProbationersAsync(int actorId, int? employeeId)
        {
            var probationers = await probationRepository.GetProbationersAsync(new GetProbationersCriteria
            {
                ActorId = actorId,
                EmployeeId = employeeId
            }).ConfigureAwait(false);

            var today = DateTime.Today;

            foreach (var probationer in probationers)
            {
                probationer.CanEdit = probationer.Settings.Any(x => x.EvaluatorId == actorId && x.FromDate <= today && x.ToDate >= today);
                probationer.CurrentProcessId = probationer.Settings.FirstOrDefault(x => x.FromDate <= today && x.ToDate >= today)?.ProcessId;
            }

            return probationers;
        }

        public async Task<IEnumerable<KPIGroupModel>> GetEvaluationDetailAsync(int employeeId)
        {
            return await probationRepository.GetEvaluationDetailAsync(new GetEvaluationDetailCriteria { EmployeeId = employeeId })
                .ConfigureAwait(false);
        }

        public async Task<CommandResult> EditSettingAsync(IEnumerable<ProbationSettingModel> settings)
        {
            return await baseService.ExecuteNonQueryAsync(new UpdateSettingCriteria
            {
                Settings = settings.ToXML().ToString()
            }).ConfigureAwait(false);
        }

        public async Task<CommandResult> EditSelfEvaluationAsync(int employeeId, string comment, IEnumerable<EditSelfEvaluationModel> selfEvaluations)
        {
            return await baseService.ExecuteNonQueryAsync(new UpdateSelfEvaluationCriteria
            {
                EmployeeId = employeeId,
                Details = selfEvaluations.ToXML().ToString(),
                Comment = comment
            }).ConfigureAwait(false);
        }

        public async Task<CommandResult> EditEvaluatorEvaluationAsync(int employeeId, string comment, IEnumerable<EditSelfEvaluationModel> evaluatorEvaluations)
        {
            return await baseService.ExecuteNonQueryAsync(new UpdateEvaluatorEvaluationCriteria
            {
                EmployeeId = employeeId,
                Details = evaluatorEvaluations.ToXML().ToString(),
                Comment = comment
            }).ConfigureAwait(false);
        }

        public async Task<CommandResult> EditReviewerEvaluationAsync(int employeeId, int finalScore, string comment)
        {
            return await baseService.ExecuteNonQueryAsync(new UpdateReviewerEvaluationCriteria
            {
                EmployeeId = employeeId,
                FinalScore = finalScore,
                Comment = comment
            }).ConfigureAwait(false);
        }
    }
}