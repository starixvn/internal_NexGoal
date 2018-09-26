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
        private readonly IPerformanceCheckInService performanceService;

        public MyCheckInController(
            ISessionService sessionService,
            IPerformanceCheckInService performanceService)
        {
            this.sessionService = sessionService;
            this.performanceService = performanceService;
        }

        [Route("Self/Performance/GetCreators")]
        public async Task<JsonResult> GetCreators()
        {
            IEnumerable<UserModel> creators = await performanceService.GetCreatorsAsync(sessionService.EmployeeId).ConfigureAwait(false);

            return Json(creators);
        }

        [Route("Self/Performance/Index")]
        public async Task<JsonResult> Index(GetPerformanceCheckInByEmployeeParam param)
        {
            param.EmployeeId = sessionService.EmployeeId;
            Tuple<IEnumerable<PerformanceRecordModel>, int> tuple =
                await performanceService.GetPerformanceCheckInByEmployeeAsync(param).ConfigureAwait(false);

            return Json(new { performanceRecords = tuple.Item1, totalItems = tuple.Item2 });
        }

        [Route("Self/Performance/Create")]
        public IActionResult Create()
        {
            return View();
        }

        [Route("Self/Performance/Create")]
        [HttpPost]
        public async Task<JsonResult> Create(CreatePerformanceRecordModel performanceRecord)
        {
            if (ModelState.IsValid)
            {
                performanceRecord.EmployeeId = sessionService.EmployeeId;
                performanceRecord.CreatorId = sessionService.EmployeeId;
                CommandResult commandResult = await performanceService.CreatePerformanceCheckInAsync(performanceRecord).ConfigureAwait(false);

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });
            return Json(new { isSuccessful = false, errors });
        }

        [Route("Self/Performance/Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            PerformanceRecordModel record = await performanceService.GetPerformanceCheckInByIdAsync(id, sessionService.EmployeeId).ConfigureAwait(false);
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

        [Route("Self/Performance/Edit")]
        [HttpPost]
        public async Task<JsonResult> Edit(EditPerformanceRecordModel performanceRecord)
        {
            if (ModelState.IsValid)
            {
                performanceRecord.EmployeeId = sessionService.EmployeeId;
                performanceRecord.CreatorId = sessionService.EmployeeId;
                CommandResult commandResult = await performanceService.EditPerformanceCheckInAsync(performanceRecord).ConfigureAwait(false);

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });
            return Json(new { isSuccessful = false, errors });
        }

        [Route("Self/Performance/GetById/{id}")]
        public async Task<JsonResult> GetById(int id)
        {
            PerformanceRecordModel record = await performanceService.GetPerformanceCheckInByIdAsync(id, sessionService.EmployeeId).ConfigureAwait(false);

            return Json(record);
        }
    }
}