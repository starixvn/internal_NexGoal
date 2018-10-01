namespace NexGoal.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetWorkLoadForPMCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "Employee_GetWorkLoadForPM";

        public override bool IsValid() => EmployeeId > 0;
    }
}