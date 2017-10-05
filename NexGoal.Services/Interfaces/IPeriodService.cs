namespace NexGoal.Services.Interfaces
{
    using System.Collections.Generic;
    using NexGoal.Core.Model;
    using NexGoal.Criteria.Period;
    using NexGoal.Criteria.Period.GenerationEvaluation;
    using NexGoal.Model.FlowManagement;
    using NexGoal.Model.Period;

    public interface IPeriodService
    {
        IEnumerable<PeriodModel> GetReadyPeriod(GetReadyPeriodCriteria criteria);

        IEnumerable<PeriodModel> GetPeriod(GetPeriodCriteria criteria);

        IEnumerable<PeriodModel> GetAllPeriod(GetAllPeriodIncludeImportCriteria criteria);

        PeriodModel GetPeriodById(int id);

        CommandResult CreatePeriod(CreatePeriodCriteria criteria);

        CommandResult UpdateBasicInformationPeriod(UpdateBasicInformationPeriodCriteria criteria);

        CommandResult UpdateStatusPeriod(UpdateStatusPeriodCriteria criteria);

        CommandResult DeletePeriod(DeletePeriodCriteria criteria);

        IEnumerable<PeriodEmployeeSettingModel> GetPeriodEmployeeSettingById(GetPeriodEmployeeSettingCriteria criteria);

        CommandResult AddEmployeeToPeriod(AddEmployeeToPeriodCriteria criteria);

        CommandResult UpdateEmployeeSettingOfPeriod(UpdateEmployeeSettingOfPeriodCriteria criteria);

        CommandResult UpdateIncludeStatusEmployeePeriod(UpdateIncludeStatusEmployeePeriodCriteria criteria);

        void ExportEmployeeSettingReport(IEnumerable<PeriodEmployeeSettingModel> employeeFlowSettings, string templatePath, string periodName);

        CommandResult GenerateEvaluation(AutoGenerateEvaluationCriteria criteria);

        void SendEmailNoticeAtBegin(int? periodId);

        IEnumerable<AutoEmployeePeriodModel> GetPeriodHasStartedProcess(GetPeriodHasStartedProcessCriteria criteria);

        IEnumerable<AutoEmployeePeriodModel> GetPeriodHasEndedProcess(GetPeriodHasEndedProcessCriteria criteria);

        IEnumerable<PeriodFlowModel> GetEmployeeSettingByPeriodId(GetEmployeeSettingByPeriodIdCriteria criteria);

        IEnumerable<PeriodFlowModel> GetEmployeeSettingOfHR(GetEmployeeSettingOfHRCriteria criteria);

        IEnumerable<FlowModel> GetProcessSettingByPeriodId(GetProcessSettingPeriodCriteria criteria);

        IEnumerable<FlowModel> GetEvaluationGlobalSettingByTargetTypeId(GetEvaluationGlobalSettingByTargetTypeIdCriteria criteria);

        CommandResult UpdateProcessSettingPeriod(UpdateProcessSettingPeriodCriteria criteria);

        bool CheckSelfProcessStarted(CheckSelfProcessStartedCriteria criteria);

        IEnumerable<PeriodModel> GetAllPeriodByEmployee(GetAllPeriodByEmployeeCriteria criteria);

        IEnumerable<PeriodModel> GetAllPeriodByEvaluator(GetAllPeriodByEvaluatorCriteria criteria);

        IEnumerable<PeriodModel> GetAllPeriodByReviewer(GetAllPeriodByReviewerCriteria criteria);

        IList<PeriodModel> GetAllPeriodIncludeImport(GetAllPeriodIncludeImportCriteria criteria);
    }
}