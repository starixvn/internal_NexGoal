namespace NexGoal.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Models;
    using NexGoal.Models.Function;
    using NexGoal.Services;
    using NexGoal.ViewModels.Function;

    public class RoleController : Controller
    {
        private readonly ICommonService commonService;
        private readonly IFunctionService functionService;

        public RoleController(
            ICommonService commonService,
            IFunctionService functionService)
        {
            this.commonService = commonService;
            this.functionService = functionService;
        }

        public async Task<IActionResult> Index(int? roleId = null)
        {
            string[] refNames = { ReferenceResource.SystemRole };
            IEnumerable<FunctionModel> functions = await functionService.GetTreeFunctionsAsync().ConfigureAwait(false);
            IEnumerable<ReferenceResourceModel> roles = await commonService.GetReferenceResourcesByNamesAsync(refNames).ConfigureAwait(false);
            var viewModel = new AssignPermissionViewModel
            {
                RoleId = roleId ?? (int)SystemRole.SystemAdmin,
                Functions = functions,
                Roles = commonService.GetSelectList(roles.OrderBy(x => x.ResourceId), nameof(ReferenceResourceModel.ResourceId), nameof(ReferenceResourceModel.Value)),
                IsHost = true
            };

            IEnumerable<FunctionModel> roleFunctions = await functionService.GetFunctionByRoleIdAsync(viewModel.RoleId).ConfigureAwait(false);
            viewModel.RolePermissions = roleFunctions.Select(x => new RolePermissionModel
            {
                RoleId = viewModel.RoleId,
                FunctionId = x.FunctionId,
                Policy = x.Policy
            });

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AssignPermission(int roleId, IEnumerable<RolePermissionModel> permissions)
        {
            CommandResult commandResult = await functionService.AssignFunctionToRoleAsync(roleId, permissions).ConfigureAwait(false);

            return Json(commandResult);
        }
    }
}