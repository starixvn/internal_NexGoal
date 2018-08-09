namespace NexGoal.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Filters;
    using NexGoal.Core.Services;
    using NexGoal.Models.UserManagement;
    using NexGoal.Services;
    using NexGoal.ViewModels.UserManagement;

    [NexGoalPermission(FunctionKey = FunctionKey.UserManagement, Policy = Policy.Read)]
    public class UserController : Controller
    {
        private readonly IUserService userService;
        private readonly ISessionService sessionService;

        public UserController(IUserService userService, ISessionService sessionService)
        {
            this.userService = userService;
            this.sessionService = sessionService;
        }

        public IActionResult Index()
        {
            var viewModel = new UserManagementViewModel
            {
                CanEditUserRoles = sessionService.HasPermissionOnFunction(FunctionKey.UserManagement, Policy.Host)
            };

            return View(viewModel);
        }

        public async Task<IActionResult> GetUsers(string keyword, int? groupId, bool? isLocal, bool isDisabledUser)
        {
            var users = await userService.GetUsers(keyword, groupId, isLocal, isDisabledUser).ConfigureAwait(false);

            return Json(users);
        }

        public async Task<IActionResult> GetUser(int employeeId)
        {
            var user = await userService.GetUser(employeeId).ConfigureAwait(false);

            return Json(user);
        }

        public async Task<IActionResult> GetRolesForUser(int employeeId)
        {
            var userRoles = await userService.GetRolesForUser(employeeId).ConfigureAwait(false);

            return Json(userRoles);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.UserManagement, Policy = Policy.Host)]
        public async Task<IActionResult> EditUserRoles(EditUserRolesViewModel model)
        {
            var result = await userService.EditUserRoles(new EditUserRolesModel
            {
                EmployeeId = model.EmployeeId,
                SelectedRoleIds = model.SelectedRoleIds,
                ModifiedBy = sessionService.EmployeeId
            }).ConfigureAwait(false);

            return Json(result);
        }
    }
}