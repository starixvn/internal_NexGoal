namespace NexGoal.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Filters;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.UserManagement;
    using NexGoal.Models.UserManagement;

    [NexGoalPermission(FunctionKey = FunctionKey.UserManagement, Policy = Policy.Read)]
    public class UserManagementController : Controller
    {
        private readonly IBaseService baseService;
        private readonly ISessionService sessionService;

        public UserManagementController(IBaseService baseService, ISessionService sessionService)
        {
            this.baseService = baseService;
            this.sessionService = sessionService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetUsers(string keyword, int? groupId, bool? isLocal, bool isDisabledUser)
        {
            var results = await baseService.GetDataAsync<UserModel>(new GetUsersCriteria
            {
                Keyword = keyword,
                GroupId = groupId,
                IsLocal = isLocal,
                IsDisabledUser = isDisabledUser
            }).ConfigureAwait(false);

            return Ok(results);
        }

        public async Task<IActionResult> GetGroups()
        {
            var results = await baseService.GetDataAsync<UserGroupModel>(new GetGroupsCriteria()).ConfigureAwait(false);

            return Ok(results);
        }

        public async Task<IActionResult> GetRolesForUser(int employeeId)
        {
            var results = await baseService.GetDataAsync<UserRoleModel>(new GetRolesForUserCriteria { EmployeeId = employeeId }).ConfigureAwait(false);

            return Ok(results);
        }

        [HttpPost]
        public async Task<IActionResult> EditUserRoles([FromBody] EditUserRolesModel model)
        {
            var results = await baseService.ExecuteNonQueryAsync(new EditUserRolesCriteria
            {
                EmployeeId = model.EmployeeId,
                SelectedRoleIds = model.SelectedRoleIds,
                ModifiedBy = sessionService.EmployeeId
            }).ConfigureAwait(false);

            return Ok(results);
        }
    }
}

public class EditUserRolesModel
{
    public int EmployeeId { get; set; }
    public string SelectedRoleIds { get; set; }
}