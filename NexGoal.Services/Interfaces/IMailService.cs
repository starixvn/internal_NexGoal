namespace NexGoal.Services.Interfaces
{
    using Model;
    using NexGoal.Model.EmployeeEPA;
    using NexGoal.Model.Period;

    public interface IMailService
    {
        void SendMailToHRStartDate(string templateEmailPath, PeriodFlowModel employeeSetting, PeriodModel period);

        void SendMailToSelfStartDate(string templateEmailPath, PeriodFlowModel employeeSetting, PeriodModel period);

        void SendMailToEvaluatorStartDate(string templateEmailPath, PeriodFlowModel employeeSetting, PeriodModel period);

        void SendMailToReviewerStartDate(string templateEmailPath, PeriodFlowModel employeeSetting, PeriodModel period);

        void SendMailToSelfRemindDate(string templateEmailPath, PeriodFlowModel employeeSetting, PeriodModel period);

        void SendMailToEvaluatorRemindDate(string templateEmailPath, PeriodFlowModel employeeSetting, PeriodModel period);

        void SendMailToReviewerRemindDate(string templateEmailPath, PeriodFlowModel employeeSetting, PeriodModel period);

        void SendMailToHRRemindDate(string templateEmailPath, PeriodFlowModel employeeSetting, PeriodModel period);

        void SendMailToNotifyChangeSetting(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName);

        void SendMailEndToHRByEmployee(string templateEmailPath, EPAModel evaluationInfo, EPAProcessModel humanResource);

        void SendMailEndToHRByPeriod(string templateEmailPath, PeriodFlowModel employeeSetting, PeriodModel period);

        void SendMailToSelfEndEvaluation(string templateEmailPath, EPAModel evaluationInfo);

        void SendMailToAllInNomination(string templateEmailPath, EOQYModel nomination, EmailNominatorModel emailNomination);
    }
}