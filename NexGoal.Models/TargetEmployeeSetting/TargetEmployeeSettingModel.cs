namespace NexGoal.Model.TargetEmployeeSetting
{
    using NexGoal.Model.Employee;
    using System.Collections.Generic;

    public class TargetEmployeeSettingModel
    {
        public IEnumerable<EmployeeModel> Employees { get; set; }

        public IEnumerable<TargetEmployeeModel> TargetEmployees { get; set; }
    }
}