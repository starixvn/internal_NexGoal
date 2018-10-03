namespace NexGoal.Criteria.Redmine
{
    using Fanex.Data.Repository;

    public class GetWorkloadByEmployeeIdCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "Employee_GetWorkload";

        public override bool IsValid() => EmployeeId > 0;
    }
}