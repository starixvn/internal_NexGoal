namespace NexGoal.Criteria.Common
{
    using Fanex.Data.Repository;

    public class GetPerformanceGradeCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "PerformanceGrade_Get";

        public override bool IsValid() => true;
    }
}