namespace NexGoal.Models.FlowSetting
{
    using System.Collections.Generic;
    using NexGoal.Models.Common;

    public class GroupFlowSettingModel : GroupBaseModel
    {
        public IList<FlowSettingModel> HumanResources { get; set; } = new List<FlowSettingModel>();
        public FlowSettingModel Evaluator { get; set; }
        public FlowSettingModel Reviewer { get; set; }
    }
}