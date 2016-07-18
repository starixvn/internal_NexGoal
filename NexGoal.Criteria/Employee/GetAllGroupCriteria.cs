namespace NexGoal.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetAllGroupCriteria : CriteriaBase
    {
        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EpaGroup_Get";
        }
    }
}