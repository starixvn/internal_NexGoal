namespace NexGoal.Criteria.PARecord
{
    using Fanex.Data.Repository;

    public class GetPeriodByActorCriteria : CriteriaBase
    {
        public int ActorId { get; set; }

        public override string GetSettingKey() => "PARecord_GetPeriodByActor";

        public override bool IsValid() => ActorId > 0;
    }
}