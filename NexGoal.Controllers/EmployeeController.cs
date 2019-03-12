namespace NexGoal.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.EmployeeRole;
    using NexGoal.Services;
    using NexGoal.Web.Core.Filters;
    using NexGoal.Web.Core.Services;

    [NexGoalPermission(FunctionKey = FunctionKey.RoleAssignment, Policy = Policy.Read)]
    public class EmployeeController : BaseController
    {
        private readonly IEmployeeService employeeService;

        public EmployeeController(
            IEmployeeService employeeService,
            ISessionService sessionService) : base(sessionService)
        {
            this.employeeService = employeeService;
        }

        public IActionResult Index()
        {
            bool isHost = sessionService.HasPermissionOnFunction(FunctionKey.RoleAssignment, Policy.Host);

            return View(isHost);
        }

        public async Task<JsonResult> GetEmployees(GetEmployeeParam param)
        {
            IEnumerable<Employee> employees = await employeeService.GetEmployeesAsync(param).ConfigureAwait(false);

            return Json(employees);
        }

        public async Task<JsonResult> GetEmployeeById(int employeeId)
        {
            Employee employee = await employeeService.GetEmployeeByIdAsync(employeeId).ConfigureAwait(false);

            return Json(employee);
        }

        public async Task<JsonResult> GetRolesByEmployeeId(int employeeId)
        {
            IEnumerable<EmployeeRoleModel> employeeRoles = await employeeService.GetRolesByEmployeeIdAsync(employeeId).ConfigureAwait(false);

            return Json(employeeRoles);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.RoleAssignment, Policy = Policy.Host)]
        public async Task<JsonResult> EditEmployeeRoles(EditEmployeeRolesModel model)
        {
            model.ModifiedBy = sessionService.EmployeeId;
            CommandResult commandResult = await employeeService.EditEmployeeRolesAsync(model).ConfigureAwait(false);

            return Json(commandResult);
        }
    }
}