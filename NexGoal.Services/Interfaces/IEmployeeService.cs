namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.Employee;
    using NexGoal.Models.EmployeeNomination;

    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeBaseModel>> GetEmployeesAsync(GetEmployeeParam param);

        Task<EmployeeBaseModel> GetEmployeeByIdAsync(int employeeId);

        Task<IEnumerable<EmployeeRoleModel>> GetRolesByEmployeeIdAsync(int employeeId);

        Task<CommandResult> EditEmployeeRolesAsync(EditEmployeeRolesModel model);

        Task<int> GetRoleAccordingToMemberAsync(int actorId, int memberId);

        Task<IEnumerable<EmployeeNominationModel>> GetNominationResultAsync(int employeeId);

        Task<IEnumerable<EmployeeNominationModel>> GetMemberNominationResultAsync(int actorId, int? employeeId = null, int? year = null);

        Task<IEnumerable<EmployeeBaseModel>> GetMemberAsync(int actorId, string keyword = null);

        Task<IEnumerable<GroupBaseModel>> GetGroupAsync(int actorId);
    }
}