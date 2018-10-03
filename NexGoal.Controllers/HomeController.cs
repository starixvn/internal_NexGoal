namespace NexGoal.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Services;
    using NexGoal.Models.Employee;
    using NexGoal.Models.EmployeeNomination;
    using NexGoal.Models.PARecord;
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

        public IActionResult WorkLoad()
        {
            return View();
        }

        public async Task<JsonResult> GetPerformanceAppraisalResult()
        {
            IEnumerable<PARecordModel> records = await employeeService.GetPerformanceAppraisalResultAsync(sessionService.EmployeeId).ConfigureAwait(false);

            return Json(records);
        }

        public async Task<JsonResult> GetWorkload()
        {
            IEnumerable<ProjectWorkloadModel> worklLoads = await employeeService.GetWorkloadAsync(sessionService.EmployeeId).ConfigureAwait(false);

            return Json(worklLoads);
        }

        public async Task<JsonResult> GetWorkloadForLeader()
        {
            IEnumerable<ProjectWorkloadModel> worklLoads = await employeeService.GetWorkloadForLeaderAsync(sessionService.EmployeeId).ConfigureAwait(false);

            return Json(worklLoads);
        }

        public async Task<JsonResult> GetMembersWorkLoad()
        {
            var worklLoads = await redmineService.GetTeamWorkload(125, new DateTime(2017, 7, 1), new DateTime(2017, 12, 31))
                .ConfigureAwait(false);

            return Json(worklLoads);
        }

        public async Task<JsonResult> GetWorkloadForPM()
        {
            IEnumerable<ProjectWorkloadModel> worklLoads = await employeeService.GetWorkloadForPMAsync(sessionService.EmployeeId).ConfigureAwait(false);

            return Json(worklLoads);
        }

        public async Task<JsonResult> GetNominationResult()
        {
            IEnumerable<EmployeeNominationModel> nominations = await employeeService.GetNominationResultAsync(sessionService.EmployeeId).ConfigureAwait(false);

            return Json(nominations);
        }
    }
}