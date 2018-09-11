namespace NexGoal.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Models.PerformanceRecord;
    using NexGoal.Services;

    public class MemberCheckInController : Controller
    {
        private readonly ISessionService sessionService;
        private readonly IPerformanceCheckInService performanceCheckInService;

        public MemberCheckInController(
            ISessionService sessionService,
            IPerformanceCheckInService performanceCheckInService)
        {
            this.sessionService = sessionService;
            this.performanceCheckInService = performanceCheckInService;
        }

        [Route("Member/PerformanceCheckIn/Create")]
        [HttpPost]
        public async Task<IActionResult> Create(CreatePerformanceRecordModel performanceRecord)
        {
            if (ModelState.IsValid)
            {
                performanceRecord.CreatorId = sessionService.EmployeeId;
                CommandResult commandResult = await performanceCheckInService.CreatePerformanceCheckInAsync(performanceRecord).ConfigureAwait(false);

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });
            return Json(new { isSuccessful = false, errors });
        }
    }
}