namespace NexGoal.Services.Interfaces
{
    using System.Collections.Generic;
    using NexGoal.Core.Model;
    using NexGoal.Criteria.Employee;
    using NexGoal.Model.Employee;
    using NexGoal.Model.Enitites;

    public interface IEmployeeService
    {
        bool CheckValidateEmployee(string employeePassword, string enterPassword);

        IEnumerable<EmployeeModel> GetAllEmployee(GetAllEmployeeCriteria criteria);

        EmployeeModel GetEmployeeById(int employeeId);

        IEnumerable<Group> GetAllGroup(GetAllGroupCriteria criteria);

        IEnumerable<JobTitle> GetAllJobTitle(GetAllJobTitleCriteria criteria);

        CommandResult AssignRoleToEmployee(AssignRoleToEmployeeCriteria criteria);

        IEnumerable<EmployeeModel> GetEmployeeByDepartmentId(GetEmployeeByDepartmentIdCriteria criteria);

        IEnumerable<EmployeeModel> GetEmployeeByGroupId(GetEmployeeByGroupIdCriteria criteria);
    }
}