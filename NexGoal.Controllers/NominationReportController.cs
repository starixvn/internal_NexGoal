namespace NexGoal.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Services;
    using NexGoal.Web.Core.Filters;

    [NexGoalPermission(FunctionKey = FunctionKey.NominationReport, Policy = Policy.Read)]
    public class NominationReportController : Controller
    {
        private readonly INominationReportService nominationReportService;

        public NominationReportController(INominationReportService nominationReportService)
        {
            this.nominationReportService = nominationReportService;
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
        public async Task<IActionResult> GetNominations(string nominationPeriodIds, string groupIds, int? employeeId, int? statusId)
        {
            return Json(await nominationReportService.GetNominations(nominationPeriodIds, groupIds, employeeId, statusId).ConfigureAwait(false));
        }
    }
}