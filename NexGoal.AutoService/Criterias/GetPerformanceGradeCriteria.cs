namespace NexGoal.AutoService.Criterias
{
    using Fanex.Data.Repository;

    public class GetPerformanceGradeCriteria : CriteriaBase
    {
        public override bool IsValid() => true;

        public override string GetSettingKey() => "GetPerformanceGrade";
    }
}