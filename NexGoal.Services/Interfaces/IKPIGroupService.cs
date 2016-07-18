namespace NexGoal.Services.Interfaces
{
    using System.Collections.Generic;
    using NexGoal.Core.Model;
    using NexGoal.Criteria.KPIGroup;
    using NexGoal.Model.KPIGroup;

    public interface IKPIGroupService
    {
        IEnumerable<KPIGroupModel> GetKPIGroup(GetKPIGroupCriteria criteria);

        KPIGroupModel GetKPIGroupById(GetKPIGroupByIdCriteria criteria);

        CommandResult CreateKPIGroup(CreateKPIGroupCriteria criteria);

        CommandResult UpdateKPIGroup(UpdateKPIGroupCriteria criteria);

        CommandResult UpdateStatusKPIGroup(UpdateStatusKPIGroupCriteria criteria);

        CommandResult DeleteKPIGroup(DeleteKPIGroupCriteria criteria);
    }
}