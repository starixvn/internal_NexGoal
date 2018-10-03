namespace NexGoal.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetMemberNominationResultCriteria : CriteriaBase
    {
        public int ActorId { get; set; }
        public int? EmployeeId { get; set; }

        public override string GetSettingKey() => "Employee_GetMemberNominationResult";

        public override bool IsValid() => ActorId > 0;
    }
}