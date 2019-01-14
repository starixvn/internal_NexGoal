namespace NexGoal.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Services;
    using NexGoal.Web.Core.Filters;
    using NexGoal.Web.Core.Services;

    [NexGoalPermission(FunctionKey = FunctionKey.NominationReport, Policy = Policy.Read)]
    public class NominationReportController : Controller
    {
        private readonly INominationReportService nominationReportService;
        private readonly ISessionService sessionService;

        public NominationReportController(INominationReportService nominationReportService, ISessionService sessionService)
        {
            this.nominationReportService = nominationReportService;
            this.sessionService = sessionService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetNominationPeriods()
        {
            return Json(await nominationReportService.GetNominationPeriodsForNominatorAsync().ConfigureAwait(false));
        }

        [HttpGet]
        public async Task<IActionResult> GetNominations(string nominationPeriodIds, string groupIds, string employeeIds, int? statusId)
        {
            return Json(await nominationReportService.GetNominations(nominationPeriodIds, groupIds, employeeIds, statusId, sessionService.EmployeeId).ConfigureAwait(false));
        }
    }
}