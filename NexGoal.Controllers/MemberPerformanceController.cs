namespace NexGoal.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Models.PerformanceRecord;
    using NexGoal.Services;
    using NexGoal.Web.Core.Filters;
    using NexGoal.Web.Core.Services;

    [NexGoalPermission(FunctionKey = FunctionKey.MemberPage, Policy = Policy.Host)]
    public class MemberPerformanceController : Controller
    {
        private readonly ISessionService sessionService;
        private readonly IPerformanceRecordService performanceService;

        public MemberPerformanceController(
            ISessionService sessionService,
            IPerformanceRecordService performanceService)
        {
            this.sessionService = sessionService;
            this.performanceService = performanceService;
        }

        [Route("Member/Performance")]
        public async Task<JsonResult> Index(int? memberId = null)
        {
            IEnumerable<PerformanceRecord> memberPerformances =
                await performanceService.GetMemberPerformanceAsync(sessionService.EmployeeId).ConfigureAwait(false);

            return Json(memberPerformances);
        }

        [Route("Member/Performance/MemmberDetail")]
        public async Task<JsonResult> MemmberDetail(int memberId)
        {
            IEnumerable<PerformanceRecord> memberPerformances =
                await performanceService.GetMemberPerformanceDetailAsync(sessionService.EmployeeId, memberId).ConfigureAwait(false);

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
                CommandResult commandResult = await performanceService.CreatePerformanceRecordAsync(performanceRecord).ConfigureAwait(false);

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });
            return Json(new { isSuccessful = false, errors });
        }

        [Route("Member/Performance/CreateDraft")]
        [HttpPost]
        public async Task<JsonResult> CreateDraft(CreatePerformanceRecordModel performanceRecord)
        {
            if (ModelState.IsValid)
            {
                performanceRecord.CreatorId = sessionService.EmployeeId;
                performanceRecord.TypeId = (int)PerformanceRecordType.PerformanceCheckin;
                CommandResult commandResult = await performanceService.CreateDraftPerformanceRecordAsync(performanceRecord).ConfigureAwait(false);

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
                CommandResult commandResult = await performanceService.EditPerformanceRecordAsync(performanceRecord).ConfigureAwait(false);

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });
            return Json(new { isSuccessful = false, errors });
        }

        [Route("Member/Performance/EditDraft")]
        [HttpPost]
        public async Task<JsonResult> EditDraft(EditPerformanceRecordModel performanceRecord)
        {
            if (ModelState.IsValid)
            {
                performanceRecord.CreatorId = sessionService.EmployeeId;
                performanceRecord.TypeId = (int)PerformanceRecordType.PerformanceCheckin;
                CommandResult commandResult = await performanceService.EditDraftPerformanceRecordAsync(performanceRecord).ConfigureAwait(false);

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });
            return Json(new { isSuccessful = false, errors });
        }

        [Route("Member/Performance/Timeline")]
        public async Task<JsonResult> GetMemberPerformanceTimeline(GetMemberPerformanceTimelineParam param)
        {
            param.ActorId = sessionService.EmployeeId;
            IEnumerable<MemberPerformanceTimelineModel> memberPerformances = await performanceService.GetMemberPerformanceTimelineAsync(param).ConfigureAwait(false);

            return Json(memberPerformances);
        }
    }
}