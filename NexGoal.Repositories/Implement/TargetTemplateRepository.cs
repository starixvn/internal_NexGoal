namespace NexGoal.Repositories.Implement
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using NexGoal.Model.KPI;
    using NexGoal.Model.KPIGroup;
    using NexGoal.Repositories.Interfaces;
    using System.Collections.Generic;

    public class TargetTemplateRepository : ITargetTemplateRepository
    {
        public IEnumerable<KPIModel> GetKPIOfTargetTemplate(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<KPIModel, KPIGroupModel, KPIModel>(
                    (kpi, gr) =>
                    {
                        kpi.KPIGroups.Add(gr);
                        return kpi;
                    },
                    "KPIGroupId",
                    criteria.GetSpParams());
            }
        }
    }
}