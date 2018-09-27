namespace NexGoal.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Models.Common;
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

        [Route("Member/PerformanceCheckIn/GetMembers")]
        public async Task<JsonResult> GetMembers()
        {
            IEnumerable<EmployeeBaseModel> members = await performanceCheckInService.GetMembersAsync(sessionService.EmployeeId).ConfigureAwait(false);

            return Json(members);
        }

        [Route("Member/PerformanceCheckIn/Create")]
        [HttpPost]
        public async Task<JsonResult> Create(CreatePerformanceRecordModel performanceRecord)
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

        [Route("Member/PerformanceCheckIn/Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            PerformanceRecordModel record = await performanceCheckInService.GetPerformanceCheckInByIdAsync(id, sessionService.EmployeeId).ConfigureAwait(false);

            if (record == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            if (record.StatusId == PerformanceRecordStatus.Closed)
            {
                return RedirectToAction("AccessDenied", "Error");
            }

            return View(id);
        }

        [Route("Member/PerformanceCheckIn/Edit")]
        [HttpPost]
        public async Task<JsonResult> Edit(EditPerformanceRecordModel performanceRecord)
        {
            if (ModelState.IsValid)
            {
                performanceRecord.EmployeeId = sessionService.EmployeeId;
                performanceRecord.CreatorId = sessionService.EmployeeId;
                CommandResult commandResult = await performanceCheckInService.EditPerformanceCheckInAsync(performanceRecord).ConfigureAwait(false);

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });
            return Json(new { isSuccessful = false, errors });
        }

        [Route("Member/PerformanceCheckIn/GetById/{id}")]
        public async Task<JsonResult> GetById(int id)
        {
            PerformanceRecordModel record = await performanceCheckInService.GetPerformanceCheckInByIdAsync(id, sessionService.EmployeeId).ConfigureAwait(false);

            return Json(record);
        }
    }
}