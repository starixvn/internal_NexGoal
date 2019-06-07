namespace NexGoal.Criteria.Probation
{
    using Fanex.Data.Repository;

    public class GetProbationersCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Probation_GetProbationers";

        public override bool IsValid() => true;
    }
}