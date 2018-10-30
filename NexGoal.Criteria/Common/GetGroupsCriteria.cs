namespace NexGoal.Criteria.Common
{
    using Fanex.Data.Repository;

    public class GetGroupsCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Group_GetAll";

        public override bool IsValid() => true;
    }
}