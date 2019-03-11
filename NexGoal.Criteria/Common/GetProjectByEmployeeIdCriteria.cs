namespace NexGoal.Criteria.Common
{
    using Fanex.Data.Repository;

    public class GetProjectByEmployeeIdCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "Project_GetByEmployeeId";

        public override bool IsValid() => true;
    }
}