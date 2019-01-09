namespace NexGoal.Models.FlowSetting
{
    using NexGoal.Models.Common;

    public class EmployeeFlowSettingModel : EmployeeBaseModel
    {
        public FlowSettingModel Evaluator { get; set; }
        public FlowSettingModel Reviewer { get; set; }
    }
}