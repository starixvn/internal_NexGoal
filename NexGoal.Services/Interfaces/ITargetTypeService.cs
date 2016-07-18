namespace NexGoal.Services.Interfaces
{
    using System.Collections.Generic;
    using NexGoal.Criteria.TargetType;
    using NexGoal.Model.TargetType;

    public interface ITargetTypeService
    {
        IEnumerable<TargetTypeModel> GetTargetType(GetTargetTypeCriteria criteria);
    }
}