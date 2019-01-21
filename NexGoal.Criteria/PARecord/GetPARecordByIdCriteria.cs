namespace NexGoal.Criteria.PARecord
{
    using Fanex.Data.Repository;

    public class GetPARecordByIdCriteria : CriteriaBase
    {
        public int PARecordId { get; set; }
        public int ActorId { get; set; }

        public override string GetSettingKey() => "PARecord_GetById";

        public override bool IsValid() => PARecordId > 0 && ActorId > 0;
    }
}