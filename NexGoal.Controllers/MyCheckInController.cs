namespace NexGoal.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Enumerations;
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
            IEnumerable<UserModel> creators = await performanceCheckInService.GetCreatorsAsync(sessionService.EmployeeId).ConfigureAwait(false);

            return Json(creators);
        }

        [Route("Self/PerformanceCheckIn/Index")]
        public async Task<IActionResult> Index(GetPerformanceCheckInByEmployeeParam param)
        {
            param.EmployeeId = sessionService.EmployeeId;
            Tuple<IEnumerable<PerformanceRecordModel>, int> tuple =
                await performanceCheckInService.GetPerformanceCheckInByEmployeeAsync(param).ConfigureAwait(false);

            return Json(new { performanceRecords = tuple.Item1, totalItems = tuple.Item2 });
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

        [Route("Self/PerformanceCheckIn/Edit/{id}")]
        public IActionResult Edit(int id)
        {
            return View(id);
        }

        [Route("Self/PerformanceCheckIn/Edit")]
        [HttpPost]
        public async Task<IActionResult> Edit(EditPerformanceRecordModel performanceRecord)
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

        [Route("Self/PerformanceCheckIn/GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
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

            return Json(record);
        }
    }
}