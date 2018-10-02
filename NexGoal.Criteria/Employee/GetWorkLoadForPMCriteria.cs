namespace NexGoal.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetWorkloadForPMCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "Employee_GetWorkloadForPM";

        public override bool IsValid() => EmployeeId > 0;
    }
}