namespace NexGoal.Models.FlowSetting
{
    using System.Collections.Generic;

    public class GroupFlowSettingModel
    {
        public int GroupId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public IList<FlowSettingModel> HumanResources { get; set; } = new List<FlowSettingModel>();
        public IList<FlowSettingModel> Evaluators { get; set; } = new List<FlowSettingModel>();
        public IList<FlowSettingModel> Reviewers { get; set; } = new List<FlowSettingModel>();
    }
}