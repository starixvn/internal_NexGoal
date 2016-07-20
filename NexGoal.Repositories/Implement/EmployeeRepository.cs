namespace NexGoal.Repositories.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using NexGoal.Model.Authentication;
    using NexGoal.Model.Department;
    using NexGoal.Model.Employee;
    using NexGoal.Model.Enitites;
    using NexGoal.Model.RoleManagement;
    using NexGoal.Repositories.Interfaces;

    public class EmployeeRepository : IEmployeeRepository
    {
        public RawEmployeeModel GetAllEmployee(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    IEnumerable<EmployeeModel> employees = multi.Read<EmployeeModel, JobTitle, Group, EmployeeModel>(
                           (emp, jb, gr) =>
                           {
                               emp.JobTitle = jb;
                               emp.Group = gr;

                               return emp;
                           },
                           "JobTitleId, GroupId");

                    IEnumerable<RawEmployeeRoleModel> roles = multi.Read<RawEmployeeRoleModel>();

                    var rawEmployee = new RawEmployeeModel
                    {
                        Employees = employees,
                        Roles = roles
                    };

                    return rawEmployee;
                }
            }
        }

        public UserSession GetEmployee(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    UserSession userSession = null;
                    IEnumerable<EmployeeModel> employees = multi.Read<EmployeeModel, JobTitle, DepartmentModel, Group, EmployeeModel>(
                           (emp, jb, dept, gr) =>
                           {
                               emp.JobTitle = jb;
                               emp.Department = dept;
                               emp.Group = gr;
                               return emp;
                           },
                           "JobTitleId, DepartmentId, GroupId");

                    if (employees != null && employees.Any())
                    {
                        EmployeeModel employee = employees.First();
                        IEnumerable<FunctionModel> functions = multi.Read<FunctionModel>();

                        userSession = new UserSession
                        {
                            EmployeeInfo = employee,
                            Functions = functions
                        };
                    }

                    return userSession;
                }
            }
        }

        public EmployeeModel GetEmployeeById(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    EmployeeModel employee = multi.Read<EmployeeModel, JobTitle, DepartmentModel, Group, EmployeeModel>(
                           (emp, jb, dept, gr) =>
                           {
                               emp.JobTitle = jb;
                               emp.Department = dept;
                               emp.Group = gr;
                               return emp;
                           },
                           "JobTitleId, DepartmentId, GroupId").FirstOrDefault();

                    if (employee != null)
                    {
                        employee.Roles = multi.Read<SystemRoleModel>().ToList();
                    }

                    return employee;
                }
            }
        }
    }
}