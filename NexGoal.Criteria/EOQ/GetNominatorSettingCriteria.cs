namespace NexGoal.Criteria.EOQ
{
    using Fanex.Data.Repository;

    public class GetNominatorSettingCriteria : CriteriaBase
    {
        public int EOQId { get; set; }
        public string Keyword { get; set; }
        public string GroupIds { get; set; }
        public int? RoleId { get; set; }
        public bool IsShowAll { get; set; }

        public override string GetSettingKey() => "EOQ_GetNominatorSetting";

        public override bool IsValid() => true;
    }
}