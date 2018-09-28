namespace NexGoal.Criteria.Common
{
    using Fanex.Data.Repository;

    public class GetGroupsCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Employee_GetGroups";

        public override bool IsValid() => true;
    }
}