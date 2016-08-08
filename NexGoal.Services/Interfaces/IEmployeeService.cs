namespace NexGoal.Services.Interfaces
{
    using System.Collections.Generic;
    using NexGoal.Core.Model;
    using NexGoal.Criteria.Employee;
    using NexGoal.Model.Authentication;
    using NexGoal.Model.Employee;
    using NexGoal.Model.Enitites;

    public interface IEmployeeService
    {
        bool CheckValidateEmployee(string employeePassword, string enterPassword);

        IEnumerable<EmployeeModel> GetAllEmployee(GetAllEmployeeCriteria criteria);

        EmployeeModel GetEmployeeById(int employeeId);

        IEnumerable<Group> GetAllGroup(GetAllGroupCriteria criteria);

        CommandResult CreateSpecialEmployee(CreateSpecialEmployeeCriteria criteria);

        CommandResult UpdateSpecialEmployee(UpdateSpecialEmployeeCriteria criteria);

        IEnumerable<JobTitle> GetAllJobTitle(GetAllJobTitleCriteria criteria);

        CommandResult ChangePassword(ChangePasswordCriteria criteria);

        CommandResult ResetPassword(ResetPasswordCriteria criteria);

        CommandResult AssignRoleToEmployee(AssignRoleToEmployeeCriteria criteria);

        IEnumerable<EmployeeModel> GetEmployeeByDepartmentId(GetEmployeeByDepartmentIdCriteria criteria);

        IEnumerable<EmployeeModel> GetEmployeeByGroupId(GetEmployeeByGroupIdCriteria criteria);

        CommandResult UpdateHasMonthlyEPA(UpdateHasMonthlyEPACriteria criteria);
    }
}