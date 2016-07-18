namespace NexGoal.Model.FlowManagement
{
    using System.Collections.Generic;
    using NexGoal.Model.Employee;

    public class EmployeeFlowSettingModel
    {
        public IEnumerable<EmployeeModel> DefaultFlowSettings { get; set; }

        public IEnumerable<EmployeeModel> PersonalFlowSettings { get; set; }
    }
}