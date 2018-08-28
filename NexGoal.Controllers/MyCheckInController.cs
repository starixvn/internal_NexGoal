namespace NexGoal.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Models.PerformanceRecord;
    using NexGoal.Models.UserManagement;
    using NexGoal.Services;

    public class MyCheckInController : Controller
    {
        private readonly ISessionService sessionService;
        private readonly IPerformanceCheckInService performanceCheckInService;

        public MyCheckInController(
            ISessionService sessionService,
            IPerformanceCheckInService performanceCheckInService)
        {
            this.sessionService = sessionService;
            this.performanceCheckInService = performanceCheckInService;
        }

        [Route("Self/PerformanceCheckIn/GetCreators")]
        public async Task<IActionResult> GetCreators()
        {
            IEnumerable<UserModel> creators = await performanceCheckInService.GetCreatorsByEmployeeAsync(sessionService.EmployeeId).ConfigureAwait(false);

            return Json(creators);
        }

        [Route("Self/PerformanceCheckIn/Index")]
        public async Task<IActionResult> Index(
            DateTime fromDate,
            DateTime toDate,
            int? creatorId = null,
            string projectIds = null)
        {
            IEnumerable<PerformanceRecordModel> performanceRecords = await performanceCheckInService.GetPerformanceCheckInByEmployeeAsync(
                sessionService.EmployeeId,
                fromDate,
                toDate,
                creatorId,
                projectIds).ConfigureAwait(false);

            return Json(performanceRecords);
        }

        [Route("Self/PerformanceCheckIn/Create")]
        public IActionResult Create()
        {
            return View();
        }

        [Route("Self/PerformanceCheckIn/Create")]
        [HttpPost]
        public async Task<IActionResult> Create(CreatePerformanceRecordModel performanceRecord)
        {
            if (ModelState.IsValid)
            {
                performanceRecord.EmployeeId = sessionService.EmployeeId;
                performanceRecord.CreatorId = sessionService.EmployeeId;
                CommandResult commandResult = await performanceCheckInService.CreatePerformanceCheckInAsync(performanceRecord).ConfigureAwait(false);

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });
            return Json(new { isSuccessful = false, errors });
        }
    }
}