namespace NexGoal.Models.FlowSetting
{
    using System.Collections.Generic;

    public class EmployeeFlowSettingModel
    {
        public int EmployeeId { get; set; }
        public string Code { get; set; }
        public string NickName { get; set; }
        public string FullName { get; set; }

        public IList<FlowSettingModel> HumanResources { get; set; } = new List<FlowSettingModel>();
        public IList<FlowSettingModel> Evaluators { get; set; } = new List<FlowSettingModel>();
        public IList<FlowSettingModel> Reviewers { get; set; } = new List<FlowSettingModel>();
    }
}