namespace NexGoal.Model.TargetEmployeeSetting
{
    using System.Collections.Generic;
    using NexGoal.Model.Employee;

    public class TargetEmployeeSettingModel
    {
        public IEnumerable<EmployeeModel> Employees { get; set; }

        public IEnumerable<TargetEmployeeModel> TargetEmployees { get; set; }
    }
}