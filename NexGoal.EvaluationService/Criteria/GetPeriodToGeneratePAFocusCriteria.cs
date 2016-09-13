namespace NexGoal.BackgroundService
{
    using Fanex.Data.Repository;

    public class GetPeriodToGeneratePAFocusCriteria : CriteriaBase
    {
        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Period_GetToGeneratePAFocus";
        }
    }
}