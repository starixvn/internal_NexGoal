namespace NexGoal.Criteria.PAFocus
{
    using Fanex.Data.Repository;

    public class GetPAFocusCriteria : CriteriaBase
    {
        public int ActorId { get; set; }
        public int PeriodId { get; set; }
        public string GroupIds { get; set; }
        public int? EmployeeId { get; set; }

        public override bool IsValid() => ActorId > 0 && PeriodId > 0;

        public override string GetSettingKey() => "PAFocus_Get";
    }
}