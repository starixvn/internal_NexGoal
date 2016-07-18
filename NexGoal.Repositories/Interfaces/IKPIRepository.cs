namespace NexGoal.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using NexGoal.Model.KPI;

    public interface IKPIRepository
    {
        IEnumerable<KPIModel> GetKPI(ICriteria criteria);
    }
}