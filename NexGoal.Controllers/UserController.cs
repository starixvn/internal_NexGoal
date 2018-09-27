namespace NexGoal.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Filters;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Models.Common;
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
            bool isHost = sessionService.HasPermissionOnFunction(FunctionKey.UserManagement, Policy.Host);

            return View(isHost);
        }

        public async Task<JsonResult> GetUsers(string keyword, int? groupId, bool? isLocal, bool isDisabledUser)
        {
            IEnumerable<EmployeeBaseModel> users = await userService.GetUsers(keyword, groupId, isLocal, isDisabledUser).ConfigureAwait(false);

            return Json(users);
        }

        public async Task<JsonResult> GetUser(int employeeId)
        {
            EmployeeBaseModel user = await userService.GetUser(employeeId).ConfigureAwait(false);

            return Json(user);
        }

        public async Task<JsonResult> GetRolesForUser(int employeeId)
        {
            IEnumerable<UserRoleModel> userRoles = await userService.GetRolesForUser(employeeId).ConfigureAwait(false);

            return Json(userRoles);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.UserManagement, Policy = Policy.Host)]
        public async Task<JsonResult> EditUserRoles(EditUserRolesViewModel model)
        {
            CommandResult commandResult = await userService.EditUserRoles(new EditUserRolesModel
            {
                EmployeeId = model.EmployeeId,
                SelectedRoleIds = model.SelectedRoleIds,
                ModifiedBy = sessionService.EmployeeId
            }).ConfigureAwait(false);

            return Json(commandResult);
        }
    }
}