namespace NexGoal.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Models.PerformanceRecord;
    using NexGoal.Services;
    using NexGoal.Web.Core.Services;

    public class MyPerformanceController : BaseController
    {
        private readonly IPerformanceRecordService performanceService;

        public MyPerformanceController(
            ISessionService sessionService,
            IPerformanceRecordService performanceService) : base(sessionService)
        {
            this.performanceService = performanceService;
        }

        [Route("Self/Performance")]
        public async Task<JsonResult> Index()
        {
            IEnumerable<PerformanceRecord> performanceRecords = await performanceService.GetPerformanceRecordByEmployeeAsync(sessionService.EmployeeId);

            return Json(performanceRecords);
        }

        [Route("Self/Performance/Timeline")]
        public async Task<JsonResult> GetPerformanceTimeline(GetPerformanceTimelineParam param)
        {
            param.EmployeeId = sessionService.EmployeeId;
            Tuple<IEnumerable<PerformanceRecord>, int> tuple = await performanceService.GetPerformanceTimelineAsync(param);

            return Json(new { performanceRecords = tuple.Item1, totalItems = tuple.Item2 });
        }

        [Route("Self/Performance/Create")]
        [HttpPost]
        public async Task<JsonResult> Create(CreatePerformanceRecordDto performanceRecord)
        {
            if (ModelState.IsValid)
            {
                performanceRecord.EmployeeIds = sessionService.EmployeeId.ToString();
                performanceRecord.CreatorId = sessionService.EmployeeId;
                performanceRecord.TypeId = (int)PerformanceRecordType.PerformanceCheckin;
                CommandResult commandResult = await performanceService.CreatePerformanceRecordAsync(performanceRecord);

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });
            return Json(new { isSuccessful = false, errors });
        }

        [Route("Self/Performance/CreateDraft")]
        [HttpPost]
        public async Task<JsonResult> CreateDraft(CreatePerformanceRecordDto performanceRecord)
        {
            if (ModelState.IsValid)
            {
                performanceRecord.EmployeeIds = sessionService.EmployeeId.ToString();
                performanceRecord.CreatorId = sessionService.EmployeeId;
                performanceRecord.TypeId = (int)PerformanceRecordType.PerformanceCheckin;
                CommandResult commandResult = await performanceService.CreateDraftPerformanceRecordAsync(performanceRecord);

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });
            return Json(new { isSuccessful = false, errors });
        }

        [Route("Self/Performance/Edit")]
        [HttpPost]
        public async Task<JsonResult> Edit(EditPerformanceRecordDto performanceRecord)
        {
            if (ModelState.IsValid)
            {
                performanceRecord.EmployeeIds = sessionService.EmployeeId.ToString();
                performanceRecord.CreatorId = sessionService.EmployeeId;
                performanceRecord.TypeId = (int)PerformanceRecordType.PerformanceCheckin;
                CommandResult commandResult = await performanceService.EditPerformanceRecordAsync(performanceRecord);

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });
            return Json(new { isSuccessful = false, errors });
        }

        [Route("Self/Performance/EditDraft")]
        [HttpPost]
        public async Task<JsonResult> EditDraft(EditPerformanceRecordDto performanceRecord)
        {
            if (ModelState.IsValid)
            {
                performanceRecord.EmployeeIds = sessionService.EmployeeId.ToString();
                performanceRecord.CreatorId = sessionService.EmployeeId;
                performanceRecord.TypeId = (int)PerformanceRecordType.PerformanceCheckin;
                CommandResult commandResult = await performanceService.EditDraftPerformanceRecordAsync(performanceRecord);

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });
            return Json(new { isSuccessful = false, errors });
        }

        [Route("Performance/Delete")]
        [HttpDelete]
        public async Task<JsonResult> Delete(int id)
        {
            CommandResult commandResult = await performanceService.DeletePerformanceRecordAsync(id, sessionService.EmployeeId);

            return Json(commandResult);
        }

        [Route("Performance/GetById/{id}")]
        public async Task<JsonResult> GetById(int id)
        {
            PerformanceRecord record = await performanceService.GetPerformanceRecordByIdAsync(id, sessionService.EmployeeId);

            return Json(record);
        }

        [Route("Performance/GetDraftById/{id}")]
        public async Task<JsonResult> GetDraftById(int id)
        {
            PerformanceRecord record = await performanceService.GetDraftPerformanceRecordByIdAsync(id, sessionService.EmployeeId);

            return Json(record);
        }

        [Route("Performance/GetByIds/{ids:minlength(1)}")]
        public async Task<JsonResult> GetByIds(string ids)
        {
            IEnumerable<PerformanceRecord> records = await performanceService.GetPerformanceRecordByIdsAsync(ids, sessionService.EmployeeId);

            return Json(records);
        }
    }
}