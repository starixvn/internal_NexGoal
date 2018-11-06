namespace NexGoal.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Services;
    using NexGoal.Models.EmployeeNomination;
    using NexGoal.Models.PARecord;
    using NexGoal.Models.Redmine;
    using NexGoal.Services;

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
            var empId = memberId ?? sessionService.EmployeeId;
            IEnumerable<ProjectWorkloadModel> workloads = await redmineService.GetWorkloadAsync(empId, periodId).ConfigureAwait(false);

            return Json(workloads);
        }

        public async Task<JsonResult> GetMembersWorkload(int periodId)
        {
            IEnumerable<TeamWorkloadModel> teamWorkloads = await redmineService.GetTeamWorkload(sessionService.EmployeeId, periodId).ConfigureAwait(false);

            return Json(teamWorkloads);
        }

        public async Task<JsonResult> GetNominationResult()
        {
            IEnumerable<EmployeeNominationModel> nominations = await employeeService.GetNominationResultAsync(sessionService.EmployeeId).ConfigureAwait(false);

            return Json(nominations);
        }
    }
}