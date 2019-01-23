namespace NexGoal.Models.FlowSetting
{
    using NexGoal.Models.Common;

    public class EmployeeFlowSettingModel : Employee
    {
        public FlowSettingModel Evaluator { get; set; }
        public FlowSettingModel Reviewer { get; set; }
    }
}