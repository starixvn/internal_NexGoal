namespace NexGoal.Services.Implement
{
    using System.Collections.Generic;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.KPIGroup;
    using NexGoal.Model.KPIGroup;
    using NexGoal.Services.Interfaces;

    public class KPIGroupService : BaseService, IKPIGroupService
    {
        public IEnumerable<KPIGroupModel> GetKPIGroup(GetKPIGroupCriteria criteria)
        {
            IEnumerable<KPIGroupModel> kpiGroups = new List<KPIGroupModel>();

            if (criteria != null && criteria.IsValid())
            {
                kpiGroups = GetData<KPIGroupModel>(criteria);
            }

            return kpiGroups;
        }

        public KPIGroupModel GetKPIGroupById(GetKPIGroupByIdCriteria criteria)
        {
            KPIGroupModel kpiGroup = new KPIGroupModel();

            if (criteria != null && criteria.IsValid())
            {
                kpiGroup = GetDataEntity<KPIGroupModel>(criteria);
            }

            return kpiGroup;
        }
    }
}