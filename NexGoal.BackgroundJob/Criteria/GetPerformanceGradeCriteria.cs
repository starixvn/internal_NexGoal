namespace NexGoal.BackgroundJob.Criteria
{
    using Fanex.Data.Repository;

    public class GetPerformanceGradeCriteria : CriteriaBase
    {
        public override bool IsValid() => true;

        public override string GetSettingKey() => "PerformanceGrade_Get";
    }
}