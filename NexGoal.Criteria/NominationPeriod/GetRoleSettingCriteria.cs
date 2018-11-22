namespace NexGoal.Criteria.NominationPeriod
{
    using Fanex.Data.Repository;

    public class GetRoleSettingCriteria : CriteriaBase
    {
        public int NominationPeriodId { get; set; }
        public string Keyword { get; set; }
        public string GroupIds { get; set; }
        public int? RoleId { get; set; }
        public bool IsShowAll { get; set; }

        public override string GetSettingKey() => "NominationPeriod_GetSetting";

        public override bool IsValid() => NominationPeriodId > 0;
    }
}