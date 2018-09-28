namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models;
    using NexGoal.Models.UserManagement;

    public interface IUserService
    {
        Task<IEnumerable<EmployeeBaseModel>> GetUsers(string keyword, int? groupId, bool? isLocal, bool isDisabledUser);

        Task<EmployeeBaseModel> GetUser(int employeeId);

        Task<IEnumerable<UserRoleModel>> GetRolesForUser(int employeeId);

        Task<CommandResult> EditUserRoles(EditUserRolesModel model);
    }
}