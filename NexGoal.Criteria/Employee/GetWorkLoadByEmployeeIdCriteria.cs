namespace NexGoal.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetWorkLoadByEmployeeIdCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "Employee_GetWorkLoad";

        public override bool IsValid() => EmployeeId > 0;
    }
}