namespace NexGoal.Repositories.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using NexGoal.Core.Model;
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
                using (IMultiResultReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    var employees = multi.Read<EmployeeModel, JobTitle, Group, EmployeeModel>(
                        (emp, jb, gr) =>
                        {
                            emp.JobTitle = jb;
                            emp.Group = gr;

                            return emp;
                        },
                        "JobTitleId, GroupId");

                    var roles = multi.Read<RawEmployeeRoleModel>();

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
                using (IMultiResultReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    UserSession userSession = null;
                    var employees = multi.Read<EmployeeModel, JobTitle, DepartmentModel, Group, EmployeeModel>(
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
                        var functions = multi.Read<FunctionModel>();

                        userSession = new UserSession
                        {
                            EmployeeId = employee.EmployeeId,
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
                using (IMultiResultReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    var employee = multi.Read<EmployeeModel, JobTitle, DepartmentModel, Group, EmployeeModel>(
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