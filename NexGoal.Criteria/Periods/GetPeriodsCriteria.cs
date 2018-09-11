namespace NexGoal.Criteria.Periods
{
    using Fanex.Data.Repository;

    public class GetPeriodsCriteria : CriteriaBase
    {
        public bool? IsActive { get; set; }

        public override string GetSettingKey() => "Period_Get";

        public override bool IsValid() => true;
    }
}