namespace NexGoal.AutoService.Criterias.PAFocus
{
    using Fanex.Data.Repository;

    public class GetPeriodStartedCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Auto_PAFocus_GetPeriodStarted";

        public override bool IsValid() => true;
    }
}