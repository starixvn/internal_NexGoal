namespace NexGoal.Criteria.EOQ
{
    public class DeleteEOQCriteria : CommandBaseCriteria
    {
        public int EOQYId { get; set; }
        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "EOQ_Delete";

        public override bool IsValid() => EOQYId > 0 && ModifiedBy > 0;
    }
}