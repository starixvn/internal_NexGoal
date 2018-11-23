namespace NexGoal.Criteria.NominationPeriod
{
    public class EditRoleSettingCriteria : CommandBaseCriteria
    {
        public int NominationPeriodId { get; set; }
        public int EmployeeId { get; set; }
        public string RoleIds { get; set; }
        public int ActorId { get; set; }

        public override string GetSettingKey() => "NominationPeriod_UpdateSetting";

        public override bool IsValid() => NominationPeriodId > 0 && ActorId > 0;
    }
}