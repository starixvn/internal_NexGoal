namespace NexGoal.Tools
{
    using Fanex.Data.Repository;

    public class GetAllPAFocusCriteria : CriteriaBase
    {
        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "NexGoalTools_GetAllPAFocus";
        }
    }
}