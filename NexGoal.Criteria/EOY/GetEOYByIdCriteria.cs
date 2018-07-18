namespace NexGoal.Criteria.EOY
{
    using Fanex.Data.Repository;

    public class GetEOYByIdCriteria : CriteriaBase
    {
        public int EOYId { get; set; }

        public override string GetSettingKey() => "EOY_GetById";

        public override bool IsValid() => EOYId > 0;
    }
}