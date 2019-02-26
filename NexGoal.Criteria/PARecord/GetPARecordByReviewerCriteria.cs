namespace NexGoal.Criteria.PARecord
{
    using Fanex.Data.Repository;

    public class GetPARecordByReviewerCriteria : CriteriaBase
    {
        public int ActorId { get; set; }

        public int PeriodId { get; set; }

        public string GroupIds { get; set; }

        public bool? IsRevised { get; set; }

        public override string GetSettingKey() => "PARecord_GetByReviewer";

        public override bool IsValid() => ActorId > 0 && PeriodId > 0;
    }
}