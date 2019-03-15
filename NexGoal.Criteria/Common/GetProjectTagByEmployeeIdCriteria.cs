namespace NexGoal.Criteria.Common
{
    using Fanex.Data.Repository;

    public class GetProjectTagByEmployeeIdCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "ProjectTag_GetByEmployeeId";

        public override bool IsValid() => true;
    }
}