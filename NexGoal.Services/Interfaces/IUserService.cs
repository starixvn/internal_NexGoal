namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Models.UserManagement;

    public interface IUserService
    {
        Task<IEnumerable<UserModel>> GetUsers(string keyword, int? groupId, bool? isLocal, bool isDisabledUser);

        Task<UserModel> GetUser(int employeeId);

        Task<IEnumerable<UserRoleModel>> GetRolesForUser(int employeeId);

        Task<CommandResult> EditUserRoles(EditUserRolesModel model);
    }
}