namespace NexGoal.BackgroundService
{
    using NexGoal.Model.EmployeeEPA;
    using NexGoal.Model.Period;
    using System.Collections.Generic;

    public interface IAutoMailService
    {
        void SendStartMail(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings);

        void SendReminderMail(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings);

        void SendEndEvaluationMailToHRByEmployee(EPAModel evaluationInfo);

        void SendEndEvaluationMailToHRByPeriod(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings);

        void SendEndEvaluationMailToSelf(EPAModel evaluationInfo);
    }
}