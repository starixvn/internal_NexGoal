namespace NexGoal.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Models.EmployeeNomination;
    using NexGoal.Models.NominationPeriod;
    using NexGoal.Services;
    using NexGoal.Web.Core.Filters;
    using NexGoal.Web.Core.Services;

    [NexGoalPermission(FunctionKey = FunctionKey.NominationReport, Policy = Policy.Read)]
    public class NominationReportController : BaseController
    {
        private readonly INominationReportService reportService;

        public NominationReportController(
            ISessionService sessionService,
            INominationReportService reportService) : base(sessionService)
        {
            this.reportService = reportService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetNominationPeriods()
        {
            IEnumerable<NominationPeriodModel> periods = await reportService.GetNominationPeriodAsync();

            return Json(periods);
        }

        [HttpGet]
        public async Task<IActionResult> GetNominations(string nominationPeriodIds, string groupIds, string employeeIds, int? statusId)
        {
            IEnumerable<NominationModel> nominations =
                await reportService.GetNominationAsync(sessionService.EmployeeId, nominationPeriodIds, groupIds, employeeIds, statusId);

            return Json(nominations);
        }
    }
}