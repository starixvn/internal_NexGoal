namespace NexGoal.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Models.EmployeeNomination;
    using NexGoal.Models.PARecord;
    using NexGoal.Models.Redmine;
    using NexGoal.Services;
    using NexGoal.Web.Core.Services;

    public class HomeController : Controller
    {
        private readonly ISessionService sessionService;
        private readonly IEmployeeNominationService nominationService;
        private readonly IPARecordService paRecordService;
        private readonly IRedmineService redmineService;

        public HomeController(
            ISessionService sessionService,
            IEmployeeNominationService nominationService,
            IPARecordService paRecordService,
            IRedmineService redmineService)
        {
            this.sessionService = sessionService;
            this.nominationService = nominationService;
            this.paRecordService = paRecordService;
            this.redmineService = redmineService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<JsonResult> GetPerformanceAppraisalResult()
        {
            IEnumerable<PerformanceAppraisalResult> records = await paRecordService.GetPerformanceAppraisalResultAsync(sessionService.EmployeeId).ConfigureAwait(false);

            return Json(records);
        }

        public async Task<JsonResult> GetWorkload(int periodId, int? memberId = null)
        {
            var employeeId = memberId ?? sessionService.EmployeeId;
            IEnumerable<ProjectWorkloadModel> workloads = await redmineService.GetWorkloadAsync(employeeId, periodId).ConfigureAwait(false);

            return Json(workloads);
        }

        public async Task<JsonResult> GetNominationResult()
        {
            IEnumerable<EmployeeNominationModel> nominations = await nominationService.GetNominationResultAsync(sessionService.EmployeeId).ConfigureAwait(false);

            return Json(nominations);
        }
    }
}