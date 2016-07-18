namespace NexGoal.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using NexGoal.Model.EPAHistory;

    public interface IEPAHistoryRepository
    {
        IList<EPAHistoryModel> GetEPAHistory(ICriteria criteria);

        IList<EPAHistoryModel> GetMonthlyEPAHistory(ICriteria criteria);
    }
}