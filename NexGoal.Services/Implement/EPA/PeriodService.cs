namespace NexGoal.Services.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Model;
    using NexGoal.Core.Services;
    using NexGoal.Core.Utilities;
    using NexGoal.Criteria.Period;
    using NexGoal.Criteria.Period.GenerationEvaluation;
    using NexGoal.Model.FlowManagement;
    using NexGoal.Model.Period;
    using NexGoal.Repositories.Implement;
    using NexGoal.Repositories.Interfaces;
    using NexGoal.Resources;
    using NexGoal.Services.Interfaces;

    public class PeriodService : BaseService, IPeriodService
    {
        private readonly IPeriodRepository _periodRepository;
        private readonly IManualMailService _manualMailService;

        public PeriodService()
            : this(null, null)
        {
        }

        public PeriodService(IPeriodRepository periodRepository, IManualMailService manualMailService)
        {
            _periodRepository = periodRepository ?? new PeriodRepository();
            _manualMailService = manualMailService ?? new ManualMailService();
        }

        public IEnumerable<PeriodModel> GetAllPeriod(GetAllPeriodIncludeImportCriteria criteria)
        {
            IEnumerable<PeriodModel> periods = new List<PeriodModel>();

            if (criteria != null && criteria.IsValid())
            {
                periods = _periodRepository.GetPeriod(criteria);
            }

            return periods;
        }

        public IEnumerable<PeriodModel> GetAllPeriodByEmployee(GetAllPeriodByEmployeeCriteria criteria)
        {
            IEnumerable<PeriodModel> periods = new List<PeriodModel>();

            if (criteria != null && criteria.IsValid())
            {
                periods = _periodRepository.GetPeriod(criteria);
            }

            return periods;
        }

        public IEnumerable<PeriodModel> GetAllPeriodByEvaluator(GetAllPeriodByEvaluatorCriteria criteria)
        {
            IEnumerable<PeriodModel> periods = new List<PeriodModel>();

            if (criteria != null && criteria.IsValid())
            {
                periods = _periodRepository.GetPeriod(criteria);
            }

            return periods;
        }

        public IEnumerable<PeriodModel> GetAllPeriodByReviewer(GetAllPeriodByReviewerCriteria criteria)
        {
            IEnumerable<PeriodModel> periods = new List<PeriodModel>();

            if (criteria != null && criteria.IsValid())
            {
                periods = _periodRepository.GetPeriod(criteria);
            }

            return periods;
        }

        public IEnumerable<PeriodModel> GetPeriod(GetPeriodCriteria criteria)
        {
            IEnumerable<PeriodModel> periods = new List<PeriodModel>();

            if (criteria != null && criteria.IsValid())
            {
                periods = _periodRepository.GetPeriod(criteria);
            }

            return periods;
        }

        public IEnumerable<PeriodModel> GetReadyPeriod(GetReadyPeriodCriteria criteria)
        {
            IEnumerable<PeriodModel> periods = new List<PeriodModel>();

            if (criteria != null && criteria.IsValid())
            {
                periods = _periodRepository.GetPeriod(criteria);
            }

            return periods;
        }

        public PeriodModel GetPeriodById(int id)
        {
            var period = new PeriodModel();
            var criteria = new GetPeriodByIdCriteria { PeriodId = id };
            if (criteria.IsValid())
            {
                period = _periodRepository.GetPeriodById(criteria);
            }

            return period;
        }

        public IEnumerable<PeriodEmployeeSettingModel> GetPeriodEmployeeSettingById(GetPeriodEmployeeSettingCriteria criteria)
        {
            IEnumerable<PeriodEmployeeSettingModel> employeeSettings = new List<PeriodEmployeeSettingModel>();

            if (criteria != null && criteria.IsValid())
            {
                employeeSettings = _periodRepository.GetPeriodEmployeeSettingById(criteria);
            }

            return employeeSettings;
        }

        public CommandResult UpdateEmployeeSettingOfPeriod(UpdateEmployeeSettingOfPeriodCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.ExecuteNonQuery(criteria);

                if (commandResult.IsSuccessful
                    && ConfigurationManager.AppSettings["SendMailWhenEditEmployeeSetting"].Equals("true", StringComparison.OrdinalIgnoreCase))
                {
                    GetAndSendEmailToAllRelatedEmployee(MailMode.NotifyChange, criteria.PeriodId, criteria.EmployeeIds, null);
                }
            }

            return commandResult;
        }

        public CommandResult GenerateEvaluation(AutoGenerateEvaluationCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.ExecuteNonQuery(criteria);
            }

            return commandResult;
        }

        public void SendEmailNoticeAtBegin(int? periodId)
        {
            var criteria = new GetPeriodHasStartedProcessCriteria
            {
                PeriodId = periodId
            };
            IEnumerable<AutoEmployeePeriodModel> startedProcess = GetPeriodHasStartedProcess(criteria);

            foreach (AutoEmployeePeriodModel periodProcess in startedProcess)
            {
                GetAndSendEmailToAllRelatedEmployee(MailMode.Start, periodProcess.PeriodId, string.Empty, periodProcess.ProcessId);
            }
        }

        public IEnumerable<PeriodFlowModel> GetEmployeeSettingByPeriodId(GetEmployeeSettingByPeriodIdCriteria criteria)
        {
            IEnumerable<PeriodFlowModel> employeeSettings = new List<PeriodFlowModel>();

            if (criteria != null && criteria.IsValid())
            {
                employeeSettings = _periodRepository.GetEmployeeSettingByPeriodId(criteria);
            }

            return employeeSettings;
        }

        public IEnumerable<PeriodFlowModel> GetEmployeeSettingOfHR(GetEmployeeSettingOfHRCriteria criteria)
        {
            IEnumerable<PeriodFlowModel> employeeSettings = new List<PeriodFlowModel>();

            if (criteria != null && criteria.IsValid())
            {
                employeeSettings = _periodRepository.GetEmployeeSettingOfHR(criteria);
            }

            return employeeSettings;
        }

        public IEnumerable<AutoEmployeePeriodModel> GetPeriodHasStartedProcess(GetPeriodHasStartedProcessCriteria criteria)
        {
            IEnumerable<AutoEmployeePeriodModel> startedPeriods = new List<AutoEmployeePeriodModel>();

            if (criteria != null && criteria.IsValid())
            {
                startedPeriods = GetData<AutoEmployeePeriodModel>(criteria);
            }

            return startedPeriods;
        }

        public IEnumerable<AutoEmployeePeriodModel> GetPeriodHasEndedProcess(GetPeriodHasEndedProcessCriteria criteria)
        {
            IEnumerable<AutoEmployeePeriodModel> startedPeriods = new List<AutoEmployeePeriodModel>();

            if (criteria != null && criteria.IsValid())
            {
                startedPeriods = GetData<AutoEmployeePeriodModel>(criteria);
            }

            return startedPeriods;
        }

        public IEnumerable<FlowModel> GetProcessSettingByPeriodId(GetProcessSettingPeriodCriteria criteria)
        {
            IEnumerable<FlowModel> processSettings = new List<FlowModel>();

            if (criteria != null && criteria.IsValid())
            {
                processSettings = GetData<FlowModel>(criteria);
            }

            return processSettings;
        }

        public IEnumerable<FlowModel> GetEvaluationGlobalSettingByTargetTypeId(GetEvaluationGlobalSettingByTargetTypeIdCriteria criteria)
        {
            IEnumerable<FlowModel> processSettings = new List<FlowModel>();

            if (criteria != null && criteria.IsValid())
            {
                processSettings = GetData<FlowModel>(criteria);
            }

            return processSettings;
        }

        public bool CheckSelfProcessStarted(CheckSelfProcessStartedCriteria criteria)
        {
            bool hasStarted = false;

            if (criteria != null && criteria.IsValid())
            {
                hasStarted = GetData<bool>(criteria).First();
            }

            return hasStarted;
        }

        public IList<PeriodModel> GetAllPeriodIncludeImport(GetAllPeriodIncludeImportCriteria criteria)
        {
            IList<PeriodModel> periods = new List<PeriodModel>();

            if (criteria != null && criteria.IsValid())
            {
                periods = GetData<PeriodModel>(criteria).ToList();
            }

            return periods;
        }

        public void ExportEmployeeSettingReport(
            IEnumerable<PeriodEmployeeSettingModel> employeeFlowSettings,
            string templatePath,
            string periodName)
        {
            string reportName = string.Format(Lang.EmployeeFlowSettingReportName, periodName);
            reportName = reportName.Replace(" ", "_");
            string reportTitle = string.Format(Lang.EmployeeFlowSettingReportTitle, periodName);
            var writer = new ExcelTemplateWriter(templatePath, reportName, Lang.EmployeeFlowSettingReportSheet);

            // Fill report title.
            writer.WriteRow(0, new object[] { reportTitle });
            writer.CopyRows(1, 1);

            // Fill report header.
            writer.CopyRows(3, 1);

            FillContentRow(employeeFlowSettings, ref writer);
            writer.SyncColumnsWidth(11);

            writer.Render();
        }

        private void FillContentRow(
         IEnumerable<PeriodEmployeeSettingModel> employeeFlowSettings,
         ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (PeriodEmployeeSettingModel employeeSetting in employeeFlowSettings)
            {
                FillRow(employeeSetting, ref writer, ref rowIndex);
            }
        }

        private void FillRow(
            PeriodEmployeeSettingModel employeeSetting,
            ref ExcelTemplateWriter writer,
            ref int rowIndex)
        {
            object record = CreateRecord(employeeSetting, ref rowIndex);
            writer.WriteRow(4, record);
        }

        private object CreateRecord(
            PeriodEmployeeSettingModel employeeSetting,
            ref int rowIndex)
        {
            var record = new
            {
                Count = rowIndex,
                Department = employeeSetting.Department != null ? employeeSetting.Department.Name : string.Empty,
                Section = employeeSetting.Group != null ? employeeSetting.Group.GroupName : string.Empty,
                ID = employeeSetting.Employee.Code,
                FullName = employeeSetting.Employee.FullName,
                NickName = employeeSetting.Employee.NickName,
                JobTitle = employeeSetting.JobTitle.JobTitleName,
                JoinedDate = employeeSetting.Employee.JoinedDate.ToString(Constants.DateFormat),
                Category = employeeSetting.Employee.Category,
                Evaluator = GetEvaluator(employeeSetting.Evaluators),
                Reviewer = GetEvaluator(employeeSetting.Reviewers),
                Form = employeeSetting.TargetTemplate != null ? employeeSetting.TargetTemplate.TargetTypeName : string.Empty
            };
            rowIndex++;

            return record;
        }

        private object GetEvaluator(IList<PeriodFlowModel> evaluators)
        {
            var evaluatorNames = new StringBuilder();

            foreach (PeriodFlowModel evaluator in evaluators)
            {
                string name = evaluator.EvaluatorInfo.NickName;
                evaluatorNames.Append(evaluatorNames.Length > 0 ? ", " + name : name);
            }

            return evaluatorNames.ToString();
        }

        private void GetAndSendEmailToAllRelatedEmployee(MailMode mailMode, int periodId, string employeeIds, int? processId)
        {
            PeriodModel period = GetPeriodById(periodId);
            IEnumerable<PeriodFlowModel> employeeSettings = GetEmployeeSettings(periodId, employeeIds, processId);

            switch (mailMode)
            {
                case MailMode.Start:
                    _manualMailService.SendStartMail(period, employeeSettings);
                    break;

                case MailMode.NotifyChange:
                    _manualMailService.SendNotifyMail(period, employeeSettings);
                    break;
            }
        }

        private IEnumerable<PeriodFlowModel> GetEmployeeSettings(int periodId, string employeeIds, int? processId)
        {
            var getEmployeeSettingCriteria = new GetEmployeeSettingByPeriodIdCriteria
            {
                PeriodId = periodId,
                EmployeeIds = employeeIds,
                ProcessId = processId
            };
            IEnumerable<PeriodFlowModel> employeeSettings = GetEmployeeSettingByPeriodId(getEmployeeSettingCriteria);

            return employeeSettings;
        }
    }
}