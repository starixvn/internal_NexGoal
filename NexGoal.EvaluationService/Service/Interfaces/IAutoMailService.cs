namespace NexGoal.BackgroundService
{
    using System.Collections.Generic;
    using NexGoal.Model.EmployeeEPA;
    using NexGoal.Model.Period;

    public interface IAutoMailService
    {
        void SendStartMail(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings);

        void SendReminderMail(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings);

        void SendEndEvaluationMailToHRByEmployee(EPAModel evaluationInfo);

        void SendEndEvaluationMailToHRByPeriod(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings);

        void SendEndEvaluationMailToSelf(EPAModel evaluationInfo);
    }
}