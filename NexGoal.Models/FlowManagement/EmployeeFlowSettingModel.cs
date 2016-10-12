namespace NexGoal.Model.FlowManagement
{
    using NexGoal.Model.Employee;
    using System.Collections.Generic;

    public class EmployeeFlowSettingModel
    {
        public IEnumerable<EmployeeModel> DefaultFlowSettings { get; set; }

        public IEnumerable<EmployeeModel> PersonalFlowSettings { get; set; }
    }
}