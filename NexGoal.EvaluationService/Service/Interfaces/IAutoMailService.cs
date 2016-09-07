namespace NexGoal.BackgroundService
{
    using System.Collections.Generic;
    using NexGoal.Model.EmployeeEPA;
    using NexGoal.Model.Period;

    public interface IAutoMailService
    {
        void SendStartMail(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings);

        void SendReminderMail(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings);

        void SendEndEvaluationMailToSelf(EPAModel evaluationInfo, float averagePerformanceScore, float disciplineScore);

        void SendEndEvaluationMailToHumanResourceByEmployee(EPAModel evaluationInfo);

        void SendEndEvaluationMailToHumanResourceByPeriod(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings);
    }
}