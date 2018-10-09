namespace NexGoal.Criteria.PAFocus
{
    using Fanex.Data.Repository;

    public class GetGroupCriteria : CriteriaBase
    {
        public int ActorId { get; set; }

        public override bool IsValid() => ActorId > 0;

        public override string GetSettingKey() => "PAFocus_GetGroup";
    }
}