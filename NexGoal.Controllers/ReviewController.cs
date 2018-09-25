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

        [NexGoalPermission(FunctionKey = FunctionKey.Review, Policy = Policy.Host)]
        public async Task<IActionResult> Edit(int id)
        {
            PARecordModel paRecord = await paRecordService.GetPARecordByIdAsync(id, sessionService.EmployeeId).ConfigureAwait(false);
            if (paRecord == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            bool isHost = sessionService.HasPermissionOnFunction(FunctionKey.Review, Policy.Host);
            ViewData["IsHost"] = isHost && paRecord.StatusId == (int)PARecordStatus.InProgress;

            return View(id);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.Review, Policy = Policy.Host)]
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

        [NexGoalPermission(FunctionKey = FunctionKey.Review, Policy = Policy.Read)]
        public async Task<IActionResult> Details(int id)
        {
            PARecordModel paRecord = await paRecordService.GetPARecordByIdAsync(id, sessionService.EmployeeId).ConfigureAwait(false);
            if (paRecord == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            return View(id);
        }

        [NexGoalPermission(FunctionKey = FunctionKey.Review, Policy = Policy.Read)]
        public async Task<IActionResult> GetPARecordById(int id)
        {
            PARecordModel paRecord = await paRecordService.GetPARecordByIdAsync(id, sessionService.EmployeeId).ConfigureAwait(false);
            if (paRecord == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            return Json(paRecord);
        }
    }
}