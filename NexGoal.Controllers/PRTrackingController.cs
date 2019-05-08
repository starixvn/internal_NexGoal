namespace NexGoal.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Services;
    using NexGoal.Web.Core.Filters;
    using NexGoal.Web.Core.Services;
    using System.Threading.Tasks;

    [NexGoalPermission(FunctionKey = FunctionKey.PRTracking, Policy = Policy.Read)]
    public class PRTrackingController : BaseController
    {
        private readonly IPRTrackingService trackingService;

        public PRTrackingController(
            ISessionService sessionService,
            IPRTrackingService trackingService) : base(sessionService)
        {
            this.trackingService = trackingService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Get(string employeeIds, bool noPR)
        {
            var result = await trackingService.GetAsync(sessionService.EmployeeId, employeeIds, noPR).ConfigureAwait(false);

            return Json(new { period = result.Item1, employees = result.Item2 });
        }

        [HttpGet]
        public async Task<IActionResult> GetGroupsAndMembers()
        {
            var result = await trackingService.GetGroupsAndMembersAsync(sessionService.EmployeeId).ConfigureAwait(false);

            return Json(new { groups = result.Item1, employees = result.Item2 });
        }

        [HttpGet]
        public async Task<IActionResult> GetDetail(int employeeId, int processId)
        {
            var result = await trackingService.GetDetailAsync(employeeId, processId).ConfigureAwait(false);

            return Json(result);
        }
    }
}