namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Models.Common;

    using NexGoal.Models.EmployeeRole;

    public interface IEmployeeService
    {
        Task<IEnumerable<Employee>> GetEmployeesAsync(GetEmployeeParam param);

        Task<Employee> GetEmployeeByIdAsync(int employeeId);

        Task<IEnumerable<EmployeeRoleModel>> GetRolesByEmployeeIdAsync(int employeeId);

        Task<CommandResult> EditEmployeeRolesAsync(EditEmployeeRolesModel model);

        Task<int> GetRoleAccordingToMemberAsync(int actorId, int memberId);

        Task<IEnumerable<Employee>> GetMemberAsync(int actorId, string keyword = null);

        Task<IEnumerable<GroupBaseModel>> GetGroupAsync(int actorId);
    }
}