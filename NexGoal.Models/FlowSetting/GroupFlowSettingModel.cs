namespace NexGoal.Models.FlowSetting
{
    using NexGoal.Models.Common;

    public class GroupFlowSettingModel : Group
    {
        public FlowSettingModel Evaluator { get; set; }
        public FlowSettingModel Reviewer { get; set; }
    }
}