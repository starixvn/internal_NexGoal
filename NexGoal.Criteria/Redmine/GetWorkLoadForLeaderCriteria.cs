namespace NexGoal.Criteria.Redmine
{
    using Fanex.Data.Repository;

    public class GetWorkloadForLeaderCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "Employee_GetWorkloadForLeader";

        public override bool IsValid() => EmployeeId > 0;
    }
}