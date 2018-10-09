namespace NexGoal.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Services;
    using NexGoal.Models.Common;
    using NexGoal.Models.PAFocus;
    using NexGoal.Services;
    using NexGoal.ViewModels.PAFocus;

    public class PAFocusController : Controller
    {
        private readonly ISessionService sessionService;
        private readonly IPAFocusService focusService;

        public PAFocusController(
            ISessionService sessionService,
            IPAFocusService focusService)
        {
            this.sessionService = sessionService;
            this.focusService = focusService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<JsonResult> GetPeriod()
        {
            IEnumerable<PeriodBaseModel> periods = await focusService.GetPeriodAsync().ConfigureAwait(false);

            return Json(periods);
        }

        public async Task<JsonResult> GetGroup()
        {
            IEnumerable<GroupBaseModel> groups = await focusService.GetGroupAsync(sessionService.EmployeeId).ConfigureAwait(false);

            return Json(groups);
        }

        public async Task<JsonResult> GetMember()
        {
            IEnumerable<EmployeeBaseModel> members = await focusService.GetMemberAsync(sessionService.EmployeeId).ConfigureAwait(false);

            return Json(members);
        }

        public async Task<JsonResult> GetPAFocus(GetPAFocusParam param)
        {
            param.ActorId = sessionService.EmployeeId;
            IEnumerable<PAFocusModel> paFocuses = await focusService.GetPAFocusAsync(param).ConfigureAwait(false);

            IEnumerable<PAFocusViewModel> result = paFocuses.GroupBy(x => new { x.Employee.GroupCode, x.Employee.GroupName },
                (key, g) => new PAFocusViewModel
                {
                    GroupCode = key.GroupCode,
                    GroupName = key.GroupName,
                    TotalBonus = g.Sum(x => !string.IsNullOrWhiteSpace(x.BonusGiven) ? int.Parse(x.BonusGiven) : 0),
                    Members = g
                });

            return Json(result);
        }
    }
}