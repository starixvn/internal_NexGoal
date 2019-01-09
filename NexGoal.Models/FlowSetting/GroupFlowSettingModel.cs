namespace NexGoal.Models.FlowSetting
{
    using NexGoal.Models.Common;

    public class GroupFlowSettingModel : GroupBaseModel
    {
        public FlowSettingModel Evaluator { get; set; }
        public FlowSettingModel Reviewer { get; set; }
    }
}