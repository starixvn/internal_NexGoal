namespace NexGoal.Criteria.Common
{
    using Fanex.Data.Repository;

    public class GetPerformanceGradeConversionCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "PerformanceGradeConversion_Get";

        public override bool IsValid() => true;
    }
}