namespace NexGoal.Services.Interfaces
{
    using System.Collections.Generic;
    using NexGoal.Criteria.KPIGroup;
    using NexGoal.Model.KPIGroup;

    public interface IKPIGroupService
    {
        IEnumerable<KPIGroupModel> GetKPIGroup(GetKPIGroupCriteria criteria);

        KPIGroupModel GetKPIGroupById(GetKPIGroupByIdCriteria criteria);
    }
}