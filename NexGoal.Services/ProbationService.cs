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
                probationer.ProcessIds = probationer.Settings.Where(x => x.EvaluatorId == actorId).Select(x => x.ProcessId);
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

        public async Task<CommandResult> EditSelfEvaluationAsync(EditSelfEvaluationModel model)
        {
            var root = ConvertToXML(model.Evaluations);

            return await baseService.ExecuteNonQueryAsync(new UpdateSelfEvaluationCriteria
            {
                EmployeeId = model.EmployeeId,
                Details = root.ToString(),
                Comment = model.Comment,
                TotalScore = model.TotalScore
            }).ConfigureAwait(false);
        }

        public async Task<CommandResult> EditEvaluatorEvaluationAsync(EditEvaluatorEvaluationModel model)
        {
            var root = ConvertToXML(model.Evaluations);

            return await baseService.ExecuteNonQueryAsync(new UpdateEvaluatorEvaluationCriteria
            {
                EmployeeId = model.EmployeeId,
                Details = root.ToString(),
                Comment = model.Comment,
                TotalScore = model.TotalScore,
                IsQualifiedByEvaluator = model.IsQualifiedByEvaluator,
                ActorId = model.ActorId,
            }).ConfigureAwait(false);
        }

        public async Task<CommandResult> EditReviewerEvaluationAsync(EditReviewerEvaluationModel model)
        {
            return await baseService.ExecuteNonQueryAsync(new UpdateReviewerEvaluationCriteria
            {
                EmployeeId = model.EmployeeId,
                IsQualified = model.IsQualified,
                Comment = model.Comment,
                ActorId = model.ActorId
            }).ConfigureAwait(false);
        }

        public async Task<CommandResult> ConfirmEvaluatorResultAsync(int employeeId, int actorId)
        {
            return await baseService.ExecuteNonQueryAsync(new ConfirmEvaluatorResultCriteria
            {
                EmployeeId = employeeId,
                ActorId = actorId
            });
        }

        public async Task<IEnumerable<ProbationerModel>> GetProbationersAsync()
        {
            return await probationRepository.GetProbationersAsync(new GetProbationersCriteria())
              .ConfigureAwait(false);
        }

        private XElement ConvertToXML(IEnumerable<EditEvaluationModel> evaluations)
        {
            XElement root = new XElement("Root");
            foreach (var evaluation in evaluations)
            {
                XElement row = new XElement("Row");
                row.SetAttributeValue(nameof(EditEvaluationModel.EmployeeId), evaluation.EmployeeId);
                row.SetAttributeValue(nameof(EditEvaluationModel.KPIId), evaluation.KPIId);
                row.SetAttributeValue(nameof(EditEvaluationModel.Point), evaluation.Point);
                root.Add(row);
            }

            return root;
        }
    }
}