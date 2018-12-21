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
        private readonly IEmployeeService employeeService;
        private readonly IRedmineService redmineService;

        public HomeController(
            ISessionService sessionService,
            IEmployeeService employeeService,
            IRedmineService redmineService)
        {
            this.sessionService = sessionService;
            this.employeeService = employeeService;
            this.redmineService = redmineService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<JsonResult> GetPerformanceAppraisalResult()
        {
            IEnumerable<PARecordModel> records = await employeeService.GetPerformanceAppraisalResultAsync(sessionService.EmployeeId).ConfigureAwait(false);

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
            IEnumerable<EmployeeNominationModel> nominations = await employeeService.GetNominationResultAsync(sessionService.EmployeeId).ConfigureAwait(false);

            return Json(nominations);
        }
    }
}