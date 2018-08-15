namespace NexGoal.Models.FlowSetting
{
    using System.Collections.Generic;

    public class GroupFlowSettingModel
    {
        public int GroupId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public IList<FlowSettingModel> HumanResources { get; set; } = new List<FlowSettingModel>();
        public FlowSettingModel Evaluator { get; set; }
        public FlowSettingModel Reviewer { get; set; }
    }
}