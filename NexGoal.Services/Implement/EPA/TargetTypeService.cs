namespace NexGoal.Services.Implement
{
    using NexGoal.Core.Services;
    using NexGoal.Criteria.TargetType;
    using NexGoal.Model.TargetType;
    using NexGoal.Services.Interfaces;
    using System.Collections.Generic;

    public class TargetTypeService : BaseService, ITargetTypeService
    {
        public IEnumerable<TargetTypeModel> GetTargetType(GetTargetTypeCriteria criteria)
        {
            IEnumerable<TargetTypeModel> kpis = new List<TargetTypeModel>();

            if (criteria != null && criteria.IsValid())
            {
                kpis = GetData<TargetTypeModel>(criteria);
            }

            return kpis;
        }
    }
}