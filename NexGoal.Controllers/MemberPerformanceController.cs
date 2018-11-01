namespace NexGoal.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Models.PerformanceRecord;
    using NexGoal.Services;

    public class MemberPerformanceController : Controller
    {
        private readonly ISessionService sessionService;
        private readonly IPerformanceRecordService performanceRecordService;

        public MemberPerformanceController(
            ISessionService sessionService,
            IPerformanceRecordService performanceRecordService)
        {
            this.sessionService = sessionService;
            this.performanceRecordService = performanceRecordService;
        }

        [Route("Member/Performance")]
        public async Task<JsonResult> Index(int? memberId = null)
        {
            IEnumerable<PerformanceRecordModel> memberPerformances =
                await performanceRecordService.GetMemberPerformanceAsync(sessionService.EmployeeId, memberId).ConfigureAwait(false);

            return Json(memberPerformances);
        }

        [Route("Member/Performance/Create")]
        [HttpPost]
        public async Task<JsonResult> Create(CreatePerformanceRecordModel performanceRecord)
        {
            if (ModelState.IsValid)
            {
                performanceRecord.CreatorId = sessionService.EmployeeId;
                performanceRecord.TypeId = (int)PerformanceRecordType.PerformanceCheckin;
                CommandResult commandResult = await performanceRecordService.CreatePerformanceRecordAsync(performanceRecord).ConfigureAwait(false);

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });
            return Json(new { isSuccessful = false, errors });
        }

        [Route("Member/Performance/Edit")]
        [HttpPost]
        public async Task<JsonResult> Edit(EditPerformanceRecordModel performanceRecord)
        {
            if (ModelState.IsValid)
            {
                performanceRecord.CreatorId = sessionService.EmployeeId;
                performanceRecord.TypeId = (int)PerformanceRecordType.PerformanceCheckin;
                CommandResult commandResult = await performanceRecordService.EditPerformanceRecordAsync(performanceRecord).ConfigureAwait(false);

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });
            return Json(new { isSuccessful = false, errors });
        }

        [Route("Member/Performance/GetById/{id}")]
        public async Task<JsonResult> GetById(int id)
        {
            PerformanceRecordModel record = await performanceRecordService.GetPerformanceRecordByIdAsync(id, sessionService.EmployeeId).ConfigureAwait(false);

            return Json(record);
        }

        [Route("Member/Performance/Timeline")]
        public async Task<JsonResult> GetMemberPerformanceTimeline(GetMemberPerformanceTimelineParam param)
        {
            param.ActorId = sessionService.EmployeeId;
            IEnumerable<MemberPerformanceTimelineModel> memberPerformances = await performanceRecordService.GetMemberPerformanceTimelineAsyn(param).ConfigureAwait(false);

            return Json(memberPerformances);
        }
    }
}