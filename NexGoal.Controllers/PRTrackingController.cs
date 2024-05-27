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
        public async Task<IActionResult> Get(string employeeIds, byte? noPR)
        {
            (var period, var employees) = await trackingService.GetAsync(sessionService.EmployeeId, employeeIds, noPR);

            return Json(new { period, employees });
        }

        [HttpGet]
        public async Task<IActionResult> GetGroupsAndMembers()
        {
            (var groups, var employees) = await trackingService.GetGroupsAndMembersAsync(sessionService.EmployeeId);

            return Json(new { groups, employees });
        }
    }
}