namespace NexGoal.Criteria.PARecord
{
    using Fanex.Data.Repository;

    public class GetGroupByActorCriteria : CriteriaBase
    {
        public int ActorId { get; set; }
        public int PeriodId { get; set; }

        public override string GetSettingKey() => "PARecord_GetGroupByActor";

        public override bool IsValid() => ActorId > 0 && PeriodId > 0;
    }
}