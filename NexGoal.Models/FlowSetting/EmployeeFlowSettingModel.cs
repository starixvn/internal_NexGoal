namespace NexGoal.Models.FlowSetting
{
    using System.Collections.Generic;
    using NexGoal.Models.Common;

    public class EmployeeFlowSettingModel : EmployeeBaseModel
    {
        public IList<FlowSettingModel> HumanResources { get; set; } = new List<FlowSettingModel>();
        public FlowSettingModel Evaluator { get; set; }
        public FlowSettingModel Reviewer { get; set; }
    }
}