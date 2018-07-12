namespace NexGoal.Criteria.EOQ
{
    using Fanex.Data.Repository;

    public class GetYearsCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "EOQ_GetYear";

        public override bool IsValid() => true;
    }
}