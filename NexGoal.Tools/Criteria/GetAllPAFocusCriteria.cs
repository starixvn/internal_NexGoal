namespace NexGoal.Tools
{
    using Fanex.Data.Repository;

    public class GetAllPAFocusCriteria : CriteriaBase
    {
        public int PAFocusIdFrom { get; set; }

        public int PAFocusIdTo { get; set; }

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