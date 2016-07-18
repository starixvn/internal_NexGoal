namespace NexGoal.Model.TargetEmployeeSetting
{
    using System.Collections.Generic;
    using NexGoal.Model.Department;

    public class TargetDepartmentSettingModel
    {
        public IEnumerable<DepartmentModel> Departments { get; set; }

        public IEnumerable<TargetDepartmentModel> TargetDepartments { get; set; }
    }
}