namespace NexGoal.Criteria.Periods
{
    using Fanex.Data.Repository;

    public class GetPeriodsCriteria : CriteriaBase
    {
        public int? TargetTypeId { get; set; }

        public bool? IsActive { get; set; }

        public override string GetSettingKey() => "Period_Get";

        public override bool IsValid() => true;
    }
}