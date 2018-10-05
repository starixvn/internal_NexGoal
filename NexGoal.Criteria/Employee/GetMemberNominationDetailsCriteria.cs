namespace NexGoal.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetMemberNominationDetailsCriteria : CriteriaBase
    {
        public int ActorId { get; set; }
        public int? NominationPeriodId { get; set; }
        public int? EmployeeId { get; set; }

        public override string GetSettingKey() => "Employee_GetMemberNominationDetails";

        public override bool IsValid() => ActorId > 0;
    }
}