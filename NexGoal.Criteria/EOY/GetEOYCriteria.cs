namespace NexGoal.Criteria.EOY
{
    using Fanex.Data.Repository;

    public class GetEOYCriteria : CriteriaBase
    {
        public int? StatusId { get; set; }

        public override string GetSettingKey() => "EOY_Get";

        public override bool IsValid() => true;
    }
}