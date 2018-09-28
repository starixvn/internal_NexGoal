namespace NexGoal.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetNominationResultCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "Employee_GetNominationResult";

        public override bool IsValid() => EmployeeId > 0;
    }
}