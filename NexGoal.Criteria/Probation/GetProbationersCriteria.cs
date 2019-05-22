namespace NexGoal.Criteria.Probation
{
    using Fanex.Data.Repository;

    public class GetProbationersCriteria : CriteriaBase
    {
        public int ActorId { get; set; }

        public override string GetSettingKey() => "Probation_GetProbationers";

        public override bool IsValid() => ActorId > 0;
    }
}