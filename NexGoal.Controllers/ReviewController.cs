namespace NexGoal.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.PARecord;
    using NexGoal.Services;
    using NexGoal.Web.Core.Filters;
    using NexGoal.Web.Core.Services;

    [NexGoalPermission(FunctionKey = FunctionKey.MemberPage, Policy = Policy.Host)]
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

        public async Task<JsonResult> GetPeriods()
        {
            IEnumerable<PeriodBaseModel> periods = await paRecordService.GetPeriodByActorAsync(sessionService.EmployeeId).ConfigureAwait(false);

            return Json(periods);
        }

        public async Task<JsonResult> GetGroups(int periodId)
        {
            IEnumerable<Group> groups = await paRecordService.GetGroupByActorAsync(sessionService.EmployeeId, periodId).ConfigureAwait(false);

            return Json(groups);
        }

        public async Task<JsonResult> GetPARecords(GetPARecordParam param)
        {
            param.ActorId = sessionService.EmployeeId;
            IEnumerable<PARecordGroupModel> paRecords = await paRecordService.GetPARecordByReviewerAsync(param).ConfigureAwait(false);

            return Json(paRecords);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditPARecordByReviewerModel paRecord)
        {
            if (ModelState.IsValid)
            {
                CommandResult commandResult = await paRecordService.EditPARecordByReviewerAsync(sessionService.EmployeeId, paRecord).ConfigureAwait(false);

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });
            return Json(new { isSuccessful = false, errors });
        }

        [HttpPost]
        public async Task<IActionResult> QuickEdit(int paRecordId, bool isRevised)
        {
            CommandResult commandResult = await paRecordService.QuickEditPARecordByReviewerAsync(sessionService.EmployeeId, paRecordId, isRevised).ConfigureAwait(false);

            return Json(commandResult);
        }
    }
}