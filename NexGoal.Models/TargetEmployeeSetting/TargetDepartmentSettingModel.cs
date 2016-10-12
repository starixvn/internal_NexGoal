namespace NexGoal.Model.TargetEmployeeSetting
{
    using NexGoal.Model.Department;
    using System.Collections.Generic;

    public class TargetDepartmentSettingModel
    {
        public IEnumerable<DepartmentModel> Departments { get; set; }

        public IEnumerable<TargetDepartmentModel> TargetDepartments { get; set; }
    }
}