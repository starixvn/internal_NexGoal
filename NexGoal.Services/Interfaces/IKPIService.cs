namespace NexGoal.Services.Interfaces
{
    using System.Collections.Generic;
    using NexGoal.Criteria.KPI;
    using NexGoal.Model.KPI;

    public interface IKPIService
    {
        IEnumerable<KPIModel> GetKPI(GetKPICriteria criteria);
    }
}