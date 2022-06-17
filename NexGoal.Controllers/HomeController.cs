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

    public class HomeController : BaseController
    {
        private readonly IEmployeeNominationService nominationService;
        private readonly IPARecordService paRecordService;
        private readonly IRedmineService redmineService;

        public HomeController(
            ISessionService sessionService,
            IEmployeeNominationService nominationService,
            IPARecordService paRecordService,
            IRedmineService redmineService) : base(sessionService)
        {
            this.nominationService = nominationService;
            this.paRecordService = paRecordService;
            this.redmineService = redmineService;
        }

        public IActionResult Index()
        {
            if (sessionService.IsInProbation)
            {
                return RedirectToAction("Index", "MyProbation");
            }

            return View();
        }

        public async Task<JsonResult> GetPerformanceAppraisalResult()
        {
            IEnumerable<PerformanceAppraisalResult> records = await paRecordService.GetPerformanceAppraisalResultAsync(sessionService.EmployeeId);

            return Json(records);
        }

        public async Task<JsonResult> GetWorkload(int periodId, int? memberId = null)
        {
            var employeeId = memberId ?? sessionService.EmployeeId;
            IEnumerable<ProjectWorkloadModel> workloads = await redmineService.GetWorkloadAsync(employeeId, periodId);

            return Json(workloads);
        }

        public async Task<JsonResult> GetNominationResult()
        {
            IEnumerable<EmployeeNominationModel> nominations = await nominationService.GetNominationResultAsync(sessionService.EmployeeId);

            return Json(nominations);
        }
    }
}