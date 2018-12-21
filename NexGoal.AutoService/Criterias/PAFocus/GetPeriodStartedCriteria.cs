namespace NexGoal.AutoService.Criterias.PAFocus
{
    using Fanex.Data.Repository;

    public class GetPeriodStartedCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "PAFocus_GetPeriodStarted";

        public override bool IsValid() => true;
    }
}