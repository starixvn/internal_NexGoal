namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.UserManagement;
    using NexGoal.Models.UserManagement;

    public class UserService : IUserService
    {
        private readonly IBaseService baseService;

        public UserService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IEnumerable<UserModel>> GetUsers(string keyword, int? groupId, bool? isLocal, bool isDisabledUser)
        {
            return await baseService.GetDataAsync<UserModel>(new GetUsersCriteria
            {
                Keyword = keyword,
                GroupId = groupId,
                IsLocal = isLocal,
                IsDisabledUser = isDisabledUser
            }).ConfigureAwait(false);
        }

        public async Task<UserModel> GetUser(int employeeId)
        {
            return await baseService.GetDataEntityAsync<UserModel>(new GetUserCriteria
            {
                EmployeeId = employeeId
            }).ConfigureAwait(false);
        }

        public async Task<IEnumerable<UserRoleModel>> GetRolesForUser(int employeeId)
        {
            return await baseService.GetDataAsync<UserRoleModel>(new GetRolesForUserCriteria { EmployeeId = employeeId }).ConfigureAwait(false);
        }

        public async Task<CommandResult> EditUserRoles(EditUserRolesModel model)
        {
            return await baseService.ExecuteNonQueryAsync(new EditUserRolesCriteria
            {
                EmployeeId = model.EmployeeId,
                SelectedRoleIds = model.SelectedRoleIds,
                ModifiedBy = model.ModifiedBy
            }).ConfigureAwait(false);
        }
    }
}