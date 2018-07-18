namespace NexGoal.Criteria.EOY
{
    public class DeleteEOYCriteria : CommandBaseCriteria
    {
        public int EOYId { get; set; }
        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "EOY_Delete";

        public override bool IsValid() => EOYId > 0 && ModifiedBy > 0;
    }
}