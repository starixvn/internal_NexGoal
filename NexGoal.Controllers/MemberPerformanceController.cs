namespace NexGoal.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Filters;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Models.PerformanceRecord;
    using NexGoal.Services;

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
            IEnumerable<PerformanceRecordModel> memberPerformances =
                await performanceService.GetMemberPerformanceAsync(sessionService.EmployeeId, memberId).ConfigureAwait(false);

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

        [Route("Member/Performance/Delete")]
        [HttpDelete]
        public async Task<JsonResult> Delete(int id)
        {
            CommandResult commandResult = await performanceService.DeletePerformanceRecordAsync(id, sessionService.EmployeeId).ConfigureAwait(false);

            return Json(commandResult);
        }

        [Route("Member/Performance/GetById/{id}")]
        public async Task<JsonResult> GetById(int id)
        {
            PerformanceRecordModel record = await performanceService.GetPerformanceRecordByIdAsync(id, sessionService.EmployeeId).ConfigureAwait(false);

            return Json(record);
        }

        [Route("Member/Performance/GetByIds/{ids:minlength(1)}")]
        public async Task<JsonResult> GetByIds(string ids)
        {
            IEnumerable<PerformanceRecordModel> records = await performanceService.GetPerformanceRecordByIdsAsync(ids, sessionService.EmployeeId).ConfigureAwait(false);

            return Json(records);
        }

        [Route("Member/Performance/Timeline")]
        public async Task<JsonResult> GetMemberPerformanceTimeline(GetMemberPerformanceTimelineParam param)
        {
            param.ActorId = sessionService.EmployeeId;
            IEnumerable<MemberPerformanceTimelineModel> memberPerformances = await performanceService.GetMemberPerformanceTimelineAsyn(param).ConfigureAwait(false);

            return Json(memberPerformances);
        }
    }
}