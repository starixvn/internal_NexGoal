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
        IEnumerable<PeriodModel> GetPeriod(GetPeriodCriteria criteria);

        PeriodModel GetPeriodById(int id);

        IEnumerable<PeriodModel> GetMonthlyPeriod(GetMonthlyPeriodCriteria criteria);

        IEnumerable<PeriodModel> GetAllMonthlyPeriod(GetAllMonthlyPeriodCriteria criteria);

        CommandResult CreatePeriod(CreatePeriodCriteria criteria);

        CommandResult AutoCreateMonthlyPeriod(AutoCreateMonthlyPeriodCriteria criteria);

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

        IEnumerable<PeriodFlowModel> GetEmployeeSettingByPeriodId(GetEmployeeSettingByPeriodIdCriteria criteria);

        IEnumerable<FlowModel> GetProcessSettingByPeriodId(GetProcessSettingPeriodCriteria criteria);

        IEnumerable<FlowModel> GetEvaluationGlobalSettingByTargetTypeId(GetEvaluationGlobalSettingByTargetTypeIdCriteria criteria);

        CommandResult UpdateProcessSettingPeriod(UpdateProcessSettingPeriodCriteria criteria);

        bool CheckSelfProcessStarted(CheckSelfProcessStartedCriteria criteria);

        PeriodModel GetInprogressSemiPeriodByEmployeeId(int employeeId);
    }
}