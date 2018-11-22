namespace NexGoal.Criteria.NominationPeriod
{
    public class DeleteNominationPeriodCriteria : CommandBaseCriteria
    {
        public int NominationPeriodId { get; set; }
        public int ActorId { get; set; }

        public override string GetSettingKey() => "NominationPeriod_Delete";

        public override bool IsValid() => NominationPeriodId > 0 && ActorId > 0;
    }
}