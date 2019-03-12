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
    using NexGoal.Web.Core.Filters;
    using NexGoal.Web.Core.Services;

    [NexGoalPermission(FunctionKey = FunctionKey.RolePermission, Policy = Policy.Read)]
    public class RoleController : BaseController
    {
        private readonly ICommonService commonService;
        private readonly IFunctionService functionService;

        public RoleController(
            ISessionService sessionService,
            ICommonService commonService,
            IFunctionService functionService) : base(sessionService)
        {
            this.commonService = commonService;
            this.functionService = functionService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<JsonResult> AssignPermission(int? roleId = null)
        {
            string[] refNames = { ReferenceResource.SystemRole };
            IEnumerable<FunctionModel> functions = await functionService.GetFunctionsAsync().ConfigureAwait(false);
            IEnumerable<ReferenceResourceModel> roles = await commonService.GetReferenceResourcesByNamesAsync(refNames).ConfigureAwait(false);
            var viewModel = new AssignPermissionViewModel
            {
                RoleId = roleId ?? (int)SystemRole.SystemAdmin,
                Functions = functions,
                Roles = commonService.GetSelectList(roles.OrderBy(x => x.ResourceId), nameof(ReferenceResourceModel.ResourceId), nameof(ReferenceResourceModel.Value)),
            };

            IEnumerable<FunctionModel> roleFunctions = await functionService.GetFunctionByRoleIdAsync(viewModel.RoleId).ConfigureAwait(false);
            MappingPermissionToFunctions(viewModel.Functions, roleFunctions);
            viewModel.IsHost = sessionService.HasPermissionOnFunction(FunctionKey.RolePermission, Policy.Host) && viewModel.RoleId != (int)SystemRole.SystemAdmin;

            return Json(viewModel);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.RolePermission, Policy = Policy.Host)]
        public async Task<JsonResult> AssignPermission(int roleId, IEnumerable<RolePermissionModel> permissions)
        {
            CommandResult commandResult = await functionService.AssignFunctionToRoleAsync(sessionService.EmployeeId, roleId, permissions).ConfigureAwait(false);

            return Json(commandResult);
        }

        private static void MappingPermissionToFunctions(IEnumerable<FunctionModel> functions, IEnumerable<FunctionModel> roleFunctions)
        {
            foreach (FunctionModel function in functions)
            {
                var roleFunction = roleFunctions.FirstOrDefault(x => x.FunctionId == function.FunctionId);
                function.Policy = roleFunction != null ? roleFunction.Policy : Policy.None;
            }
        }
    }
}