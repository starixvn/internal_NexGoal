namespace NexGoal.Criteria.EOQ
{
    public class EditNominatorSettingCriteria : CommandBaseCriteria
    {
        public int EOQId { get; set; }
        public int EmployeeId { get; set; }
        public string RoleIds { get; set; }
        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "EOQ_UpdateNominatorSetting";

        public override bool IsValid() => EOQId > 0 && ModifiedBy > 0;
    }
}