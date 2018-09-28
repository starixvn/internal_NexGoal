namespace NexGoal.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetEmployeeByIdCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "Employee_GetById";

        public override bool IsValid() => EmployeeId > 0;
    }
}