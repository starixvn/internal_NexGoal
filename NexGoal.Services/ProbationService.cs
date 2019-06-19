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
    using System.Xml.Linq;

    public class ProbationService : IProbationService
    {
        private readonly IProbationRepository probationRepository;

        private readonly IBaseService baseService;

        public ProbationService(IProbationRepository probationRepository, IBaseService baseService)
        {
            this.probationRepository = probationRepository;
            this.baseService = baseService;
        }

        public async Task<IEnumerable<ProbationEvaluationModel>> GetProbationEvaluationsAsync(int actorId, int? employeeId)
        {
            var probationers = await probationRepository.GetProbationEvaluationsAsync(new GetEvaluationsCriteria
            {
                ActorId = actorId,
                EmployeeId = employeeId
            }).ConfigureAwait(false);

            var today = DateTime.Today;

            foreach (var probationer in probationers)
            {
                probationer.CanEdit = probationer.Settings.Any(x => x.FromDate <= today && x.ToDate >= today && x.EvaluatorId == actorId);
            }

            return probationers;
        }

        public async Task<IEnumerable<KPIGroupModel>> GetEvaluationDetailAsync(int employeeId)
        {
            return await probationRepository.GetEvaluationDetailAsync(new GetEvaluationDetailCriteria { EmployeeId = employeeId })
                .ConfigureAwait(false);
        }

        public async Task<HRMModel> GetHRM(int employeeId)
        {
            return await probationRepository.GetHRM(new GetHRMCriteria { EmployeeId = employeeId })
                .ConfigureAwait(false);
        }

        public async Task<CommandResult> EditSettingAsync(IEnumerable<EditProbationSettingModel> settings, int employeeId, int actorId)
        {
            return await baseService.ExecuteNonQueryAsync(new UpdateSettingCriteria
            {
                Settings = settings.ToXML().ToString(),
                EmployeeId = employeeId,
                ActorId = actorId
            }).ConfigureAwait(false);
        }

        public async Task<CommandResult> EditSelfEvaluationAsync(int employeeId, string comment, IEnumerable<EditEvaluationModel> selfEvaluations)
        {
            XElement root = new XElement("Root");
            foreach (var evaluation in selfEvaluations)
            {
                XElement row = new XElement("Row");
                row.SetAttributeValue(nameof(EditEvaluationModel.EmployeeId), evaluation.EmployeeId);
                row.SetAttributeValue(nameof(EditEvaluationModel.KPIId), evaluation.KPIId);
                row.SetAttributeValue(nameof(EditEvaluationModel.Point), evaluation.Point);
                root.Add(row);
            }

            return await baseService.ExecuteNonQueryAsync(new UpdateSelfEvaluationCriteria
            {
                EmployeeId = employeeId,
                Details = root.ToString(),
                Comment = comment
            }).ConfigureAwait(false);
        }

        public async Task<CommandResult> EditEvaluatorEvaluationAsync(int employeeId, string comment, IEnumerable<EditEvaluationModel> evaluatorEvaluations)
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

        public async Task<IEnumerable<ProbationerModel>> GetProbationersAsync()
        {
            return await probationRepository.GetProbationersAsync(new GetProbationersCriteria())
              .ConfigureAwait(false);
        }
    }
}