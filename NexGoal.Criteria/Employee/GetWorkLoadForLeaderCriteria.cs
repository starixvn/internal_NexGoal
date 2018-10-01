namespace NexGoal.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetWorkLoadForLeaderCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "Employee_GetWorkLoadForLeader";

        public override bool IsValid() => EmployeeId > 0;
    }
}