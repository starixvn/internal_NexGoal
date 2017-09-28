namespace NexGoal.Repositories.Implement
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using NexGoal.Model.Department;
    using NexGoal.Model.Employee;
    using NexGoal.Model.Enitites;
    using NexGoal.Model.TargetEmployeeSetting;
    using NexGoal.Model.TargetTemplate;
    using NexGoal.Repositories.Interfaces;

    public class TargetEmployeeSettingRepository : ITargetEmployeeSettingRepository
    {
        public TargetEmployeeSettingModel GetTargetEmployeeSetting(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    var employees = multi.Read<EmployeeModel, Group, EmployeeModel>(
                        (emp, gr) =>
                        {
                            emp.Group = gr;
                            return emp;
                        },
                        "GroupId");

                    var targetEmployees = multi.Read<TargetEmployeeModel, TargetTemplateModel, TargetEmployeeModel>(
                        (tarEmp, tar) =>
                        {
                            tarEmp.RawTargetTemplate = tar;
                            return tarEmp;
                        },
                        "TargetTemplateId");

                    var targetEmployeeSetting = new TargetEmployeeSettingModel
                    {
                        Employees = employees,
                        TargetEmployees = targetEmployees
                    };

                    return targetEmployeeSetting;
                }
            }
        }

        public TargetDepartmentSettingModel GetTargetDepartmentSetting(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    var deparments = multi.Read<DepartmentModel>();
                    var targetDepartments = multi.Read<TargetDepartmentModel, TargetTemplateModel, TargetDepartmentModel>(
                        (tarEmp, tar) =>
                        {
                            tarEmp.RawTargetTemplate = tar;
                            return tarEmp;
                        },
                        "TargetTemplateId");

                    var targetDepartmentSetting = new TargetDepartmentSettingModel
                    {
                        Departments = deparments,
                        TargetDepartments = targetDepartments
                    };

                    return targetDepartmentSetting;
                }
            }
        }
    }
}