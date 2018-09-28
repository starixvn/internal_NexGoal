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
    using NexGoal.Models.Employee;
    using NexGoal.Services;
    using NexGoal.ViewModels.Employee;

    [NexGoalPermission(FunctionKey = FunctionKey.EmployeeManagement, Policy = Policy.Read)]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService employeeService;
        private readonly ISessionService sessionService;

        public EmployeeController(IEmployeeService employeeService, ISessionService sessionService)
        {
            this.employeeService = employeeService;
            this.sessionService = sessionService;
        }

        public IActionResult Index()
        {
            bool isHost = sessionService.HasPermissionOnFunction(FunctionKey.EmployeeManagement, Policy.Host);

            return View(isHost);
        }

        public async Task<JsonResult> GetEmployees(string keyword, int? groupId, bool? isLocal, bool isDisabled)
        {
            IEnumerable<EmployeeBaseModel> employees = await employeeService.GetEmployeesAsync(keyword, groupId, isLocal, isDisabled).ConfigureAwait(false);

            return Json(employees);
        }

        public async Task<JsonResult> GetEmployeeById(int employeeId)
        {
            EmployeeBaseModel employee = await employeeService.GetEmployeeByIdAsync(employeeId).ConfigureAwait(false);

            return Json(employee);
        }

        public async Task<JsonResult> GetRolesByEmployeeId(int employeeId)
        {
            IEnumerable<EmployeeRoleModel> employeeRoles = await employeeService.GetRolesByEmployeeIdAsync(employeeId).ConfigureAwait(false);

            return Json(employeeRoles);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.EmployeeManagement, Policy = Policy.Host)]
        public async Task<JsonResult> EditEmployeeRoles(EditEmployeeRolesViewModel model)
        {
            CommandResult commandResult = await employeeService.EditEmployeeRolesAsync(new EditEmployeeRolesModel
            {
                EmployeeId = model.EmployeeId,
                SelectedRoleIds = model.SelectedRoleIds,
                ModifiedBy = sessionService.EmployeeId
            }).ConfigureAwait(false);

            return Json(commandResult);
        }
    }
}