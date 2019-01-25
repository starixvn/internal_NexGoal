namespace NexGoal.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.PMSetting;
    using NexGoal.Services;
    using NexGoal.Web.Core.Filters;
    using NexGoal.Web.Core.Services;

    [NexGoalPermission(FunctionKey = FunctionKey.PMAssignment, Policy = Policy.Host)]
    public class PMSettingController : Controller
    {
        private readonly IPMSettingService pmSettingService;
        private readonly ISessionService sessionService;

        public PMSettingController(IPMSettingService pmSettingService, ISessionService sessionService)
        {
            this.pmSettingService = pmSettingService;
            this.sessionService = sessionService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetSetting(string projectManagerIds, int? employeeId)
        {
            IEnumerable<MemberModel> members = await pmSettingService.GetSettingAsync(projectManagerIds, employeeId).ConfigureAwait(false);

            return Json(members);
        }

        [HttpPost]
        public async Task<IActionResult> AddMembers(int projectManagerId, string memberIds)
        {
            CommandResult commandResult = await pmSettingService.AddMembers(projectManagerId, memberIds, sessionService.EmployeeId).ConfigureAwait(false);

            return Json(commandResult);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveMember(int projectManagerId, int memberId)
        {
            CommandResult commandResult = await pmSettingService.RemoveMember(projectManagerId, memberId, sessionService.EmployeeId).ConfigureAwait(false);

            return Json(commandResult);
        }

        [HttpGet]
        public async Task<IActionResult> GetProjectManagers()
        {
            IEnumerable<Employee> projectManagers = await pmSettingService.GetProjectManagerAsync().ConfigureAwait(false);

            return Json(projectManagers);
        }
    }
}