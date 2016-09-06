namespace NexGoal.Criteria
{
    using Fanex.Data.Repository;

    public class GetEOQCriteria : CriteriaBase
    {
        public int? Year { get; set; }

        public int? StatusId { get; set; }

        public override string GetSettingKey()
        {
            return "EOQ_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}