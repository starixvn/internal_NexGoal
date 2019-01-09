namespace NexGoal.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
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
        public async Task<IActionResult> GetSetting(string pmIds, int? employeeId)
        {
            var result = await pmSettingService.GetSettingAsync(pmIds, employeeId).ConfigureAwait(false);

            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddMembers(int projectManagerId, string memberIds)
        {
            return Json(await pmSettingService.AddMembers(projectManagerId, memberIds, sessionService.EmployeeId).ConfigureAwait(false));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveMember(int projectManagerId, int memberId)
        {
            return Json(await pmSettingService.RemoveMember(projectManagerId, memberId, sessionService.EmployeeId).ConfigureAwait(false));
        }

        [HttpGet]
        public async Task<IActionResult> GetProjectManagers()
        {
            var result = await pmSettingService.GetProjectManagersAsync().ConfigureAwait(false);

            return Json(result);
        }
    }
}