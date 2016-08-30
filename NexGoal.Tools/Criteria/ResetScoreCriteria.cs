namespace NexGoal.Tools
{
    using Fanex.Data.Repository;

    public class ResetScoreCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "NexGoalTools_ResetScore";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}