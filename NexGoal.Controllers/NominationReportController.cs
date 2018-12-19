namespace NexGoal.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Filters;
    using NexGoal.Services;

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
        public async Task<IActionResult> GetNominationPeriods(bool isQuarter)
        {
            return Json(await nominationReportService.GetNominationPeriodsForNominatorAsync(isQuarter).ConfigureAwait(false));
        }
    }
}