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
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    public class ProbationService : IProbationService
    {
        private readonly IProbationRepository probationRepository;
        private readonly ICryptoService cryptoService;
        private readonly IBaseService baseService;
        private readonly ICommonService commonService;

        public ProbationService(
            IProbationRepository probationRepository,
            IBaseService baseService,
            ICryptoService cryptoService,
            ICommonService commonService)
        {
            this.probationRepository = probationRepository;
            this.baseService = baseService;
            this.cryptoService = cryptoService;
            this.commonService = commonService;
        }

        public async Task<IEnumerable<ProbationEvaluationModel>> GetProbationEvaluationsAsync(int actorId, int? employeeId)
        {
            var probationers = await probationRepository.GetProbationEvaluationsAsync(new GetEvaluationsCriteria
            {
                ActorId = actorId,
                EmployeeId = employeeId
            });

            var today = DateTimeOffset.Now.DateTime;

            foreach (var probationer in probationers)
            {
                probationer.CanEdit = probationer.Settings.Any(x => x.FromDate <= today && x.ToDate >= today && x.EvaluatorId == actorId);
                probationer.ProcessIds = probationer.Settings.Where(x => x.EvaluatorId == actorId).Select(x => x.ProcessId);
                probationer.SelfScore = cryptoService.Decrypt(probationer.SelfScore);
                probationer.EvaluatorScore = cryptoService.Decrypt(probationer.EvaluatorScore);
            }

            return probationers;
        }

        public async Task<IEnumerable<KPIGroupModel>> GetEvaluationDetailAsync(int employeeId)
        {
            var criteria = new GetEvaluationDetailCriteria { EmployeeId = employeeId };
            (var groups, var details) = await baseService.FetchMultipleAsync<KPIGroupModel, EncryptedProbationEvaluationDetailModel>(criteria);

            foreach (var group in groups)
            {
                group.Details = details.Where(x => x.KPIGroupId == group.KPIGroupId).Select(x => new ProbationEvaluationDetailModel
                {
                    KPIId = x.KPIId,
                    KPIName = x.KPIName,
                    KPIGroupId = x.KPIGroupId,
                    EmployeeId = x.EmployeeId,
                    SelfPoint = string.IsNullOrWhiteSpace(x.SelfPoint) ? (decimal?)null : decimal.Parse(cryptoService.Decrypt(x.SelfPoint)),
                    EvaluatorPoint = string.IsNullOrWhiteSpace(x.EvaluatorPoint) ? (decimal?)null : decimal.Parse(cryptoService.Decrypt(x.EvaluatorPoint))
                });
            }

            return groups;
        }

        public async Task<HRMModel> GetHRM(int employeeId)
        {
            return await probationRepository.GetHRM(new GetHRMCriteria { EmployeeId = employeeId });
        }

        public async Task<CommandResult> EditSettingAsync(IEnumerable<EditProbationSettingModel> settings, int employeeId, int actorId)
        {
            return await baseService.ExecuteNonQueryAsync(new UpdateSettingCriteria
            {
                Settings = settings.ToXML().ToString(),
                EmployeeId = employeeId,
                ActorId = actorId
            });
        }

        public async Task<CommandResult> EditSelfEvaluationAsync(EditSelfEvaluationModel model)
        {
            var root = ConvertToXML(model.Evaluations);

            return await baseService.ExecuteNonQueryAsync(new UpdateSelfEvaluationCriteria
            {
                EmployeeId = model.EmployeeId,
                Details = root.ToString(),
                Comment = commonService.RemoveInvalidXmlChars(model.Comment),
                TotalScore = cryptoService.Encrypt(model.TotalScore.ToString())
            });
        }

        public async Task<CommandResult> EditEvaluatorEvaluationAsync(EditEvaluatorEvaluationModel model)
        {
            var root = ConvertToXML(model.Evaluations);

            return await baseService.ExecuteNonQueryAsync(new UpdateEvaluatorEvaluationCriteria
            {
                EmployeeId = model.EmployeeId,
                Details = root.ToString(),
                Comment = commonService.RemoveInvalidXmlChars(model.Comment),
                TotalScore = cryptoService.Encrypt(model.TotalScore.ToString()),
                IsQualifiedByEvaluator = model.IsQualifiedByEvaluator,
                ActorId = model.ActorId,
            });
        }

        public async Task<CommandResult> EditReviewerEvaluationAsync(EditReviewerEvaluationModel model)
        {
            return await baseService.ExecuteNonQueryAsync(new UpdateReviewerEvaluationCriteria
            {
                EmployeeId = model.EmployeeId,
                IsQualified = model.IsQualified,
                Comment = commonService.RemoveInvalidXmlChars(model.Comment),
                ActorId = model.ActorId
            });
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
            return await probationRepository.GetProbationersAsync(new GetProbationersCriteria());
        }

        public async Task<IEnumerable<BaseProbationerModel>> GetProbationersNotInEvaluationAsync()
        {
            return await baseService.GetDataAsync<BaseProbationerModel>(new GetProbationersNotInEvaluationCriteria());
        }

        public async Task<CommandResult> AddNewEmployeesAsync(IEnumerable<int> employeeIds)
        {
            return await baseService.ExecuteNonQueryAsync(new AddNewEmployeesCriteria
            {
                EmployeeIds = string.Join(',', employeeIds)
            });
        }

        private XElement ConvertToXML(IEnumerable<EditEvaluationModel> evaluations)
        {
            XElement root = new XElement("Root");
            foreach (var evaluation in evaluations)
            {
                XElement row = new XElement("Row");
                row.SetAttributeValue(nameof(EditEvaluationModel.EmployeeId), evaluation.EmployeeId);
                row.SetAttributeValue(nameof(EditEvaluationModel.KPIId), evaluation.KPIId);
                row.SetAttributeValue(nameof(EditEvaluationModel.Point), cryptoService.Encrypt(evaluation.Point.ToString(CultureInfo.InvariantCulture)));
                root.Add(row);
            }

            return root;
        }
    }
}