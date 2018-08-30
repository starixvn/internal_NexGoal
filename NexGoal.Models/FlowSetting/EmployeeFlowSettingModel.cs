namespace NexGoal.Models.FlowSetting
{
    using System.Collections.Generic;
    using NexGoal.Models.UserManagement;

    public class EmployeeFlowSettingModel : UserModel
    {
        public IList<FlowSettingModel> HumanResources { get; set; } = new List<FlowSettingModel>();
        public FlowSettingModel Evaluator { get; set; }
        public FlowSettingModel Reviewer { get; set; }
    }
}