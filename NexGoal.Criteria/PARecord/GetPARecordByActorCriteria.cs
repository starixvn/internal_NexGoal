namespace NexGoal.Criteria.PARecord
{
    using Fanex.Data.Repository;

    public class GetPARecordByActorCriteria : CriteriaBase
    {
        public int ActorId { get; set; }
        public int PeriodId { get; set; }
        public string GroupIds { get; set; }

        public override string GetSettingKey() => "PARecord_GetByActor";

        public override bool IsValid() => ActorId > 0 && PeriodId > 0;
    }
}