namespace NexGoal.Repositories.Interfaces
{
    using Fanex.Data.Repository;
    using NexGoal.Model.Period;
    using System.Collections.Generic;

    public interface IPeriodRepository
    {
        IEnumerable<PeriodModel> GetPeriod(ICriteria criteria);

        PeriodModel GetPeriodById(ICriteria criteria);

        IEnumerable<PeriodEmployeeSettingModel> GetPeriodEmployeeSettingById(ICriteria criteria);

        IEnumerable<PeriodFlowModel> GetEmployeeSettingByPeriodId(ICriteria criteria);

        IEnumerable<PeriodFlowModel> GetEmployeeSettingOfHR(ICriteria criteria);
    }
}