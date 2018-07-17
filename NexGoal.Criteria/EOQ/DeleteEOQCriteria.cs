namespace NexGoal.Criteria.EOQ
{
    public class DeleteEOQCriteria : CommandBaseCriteria
    {
        public int EOQId { get; set; }
        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "EOQ_Delete";

        public override bool IsValid() => EOQId > 0 && ModifiedBy > 0;
    }
}