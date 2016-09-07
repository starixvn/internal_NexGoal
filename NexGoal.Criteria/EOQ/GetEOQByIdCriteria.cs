namespace NexGoal.Criteria
{
    using Fanex.Data.Repository;

    public class GetEOQByIdCriteria : CriteriaBase
    {
        public int EOQYId { get; set; }

        public override string GetSettingKey()
        {
            return "EOQ_GetById";
        }

        public override bool IsValid()
        {
            return EOQYId > 0;
        }
    }
}