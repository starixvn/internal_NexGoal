namespace NexGoal.Criteria.PARecord
{
    public class EditPARecordByReviewerCriteria : CommandBaseCriteria
    {
        public int PARecordId { get; set; }
        public int ActorId { get; set; }
        public string GradeId { get; set; }
        public string Comment { get; set; }

        public override string GetSettingKey() => "PARecord_UpdateByReviewer";

        public override bool IsValid() => PARecordId > 0 && ActorId > 0;
    }
}