namespace NexGoal.Criteria.NominationPeriod
{
    using Fanex.Data.Repository;

    public class GetNominationPeriodCriteria : CriteriaBase
    {
        public bool? IsCompleted { get; set; }

        public override string GetSettingKey() => "NominationPeriod_Get";

        public override bool IsValid() => true;
    }
}