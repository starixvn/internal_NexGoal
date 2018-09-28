namespace NexGoal.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetPerformanceAppraisalResult : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "Employee_GetPerformanceAppraisalResult";

        public override bool IsValid() => EmployeeId > 0;
    }
}