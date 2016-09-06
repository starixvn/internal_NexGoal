namespace NexGoal.Criteria
{
    using Fanex.Data.Repository;

    public class DeleteEOQCriteria : CriteriaBase
    {
        public int EOQId { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "EOQ_Delete";
        }

        public override bool IsValid()
        {
            return EOQId > 0 && ModifiedBy > 0;
        }
    }
}