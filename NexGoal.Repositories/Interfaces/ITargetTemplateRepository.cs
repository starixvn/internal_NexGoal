namespace NexGoal.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using NexGoal.Model.KPI;

    public interface ITargetTemplateRepository
    {
        IEnumerable<KPIModel> GetKPIOfTargetTemplate(ICriteria criteria);
    }
}