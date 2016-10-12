namespace NexGoal.Services.Interfaces
{
    using NexGoal.Model.Period;
    using System.Collections.Generic;

    public interface IManualMailService
    {
        void SendStartMail(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings);

        void SendNotifyMail(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings);
    }
}