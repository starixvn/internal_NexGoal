namespace NexGoal.Services.Interfaces
{
    using NexGoal.Criteria.TargetType;
    using NexGoal.Model.TargetType;
    using System.Collections.Generic;

    public interface ITargetTypeService
    {
        IEnumerable<TargetTypeModel> GetTargetType(GetTargetTypeCriteria criteria);
    }
}