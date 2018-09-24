namespace NexGoal.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Services;
    using NexGoal.Models.PARecord;
    using NexGoal.Models.Period;
    using NexGoal.Models.UserManagement;
    using NexGoal.Services;

    public class ReviewController : Controller
    {
        private readonly ISessionService sessionService;
        private readonly IPARecordService paRecordService;

        public ReviewController(
            ISessionService sessionService,
            IPARecordService paRecordService)
        {
            this.sessionService = sessionService;
            this.paRecordService = paRecordService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<JsonResult> GetPeriods()
        {
            IEnumerable<PeriodModel> periods = await paRecordService.GetPeriodByActorAsync(sessionService.EmployeeId).ConfigureAwait(false);

            return Json(periods);
        }

        public async Task<JsonResult> GetGroups(int periodId)
        {
            IEnumerable<UserGroupModel> groups = await paRecordService.GetGroupByActorAsync(sessionService.EmployeeId, periodId).ConfigureAwait(false);

            return Json(groups);
        }

        public async Task<JsonResult> GetPARecords(GetPARecordParam param)
        {
            param.ActorId = sessionService.EmployeeId;
            IEnumerable<PARecordModel> paRecords = await paRecordService.GetPARecordByActorAsync(param).ConfigureAwait(false);

            return Json(paRecords);
        }
    }
}