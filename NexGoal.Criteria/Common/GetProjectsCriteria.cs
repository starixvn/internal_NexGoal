namespace NexGoal.Criteria.Common
{
    using Fanex.Data.Repository;

    public class GetProjectsCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Project_GetAll";

        public override bool IsValid() => true;
    }
}