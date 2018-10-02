namespace NexGoal.Controllers
{
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

        public HomeController(
            ISessionService sessionService,
            IEmployeeService employeeService)
        {
            this.sessionService = sessionService;
            this.employeeService = employeeService;
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
            IEnumerable<EmployeeWorkLoadModel> worklLoads = await employeeService.GetWorkloadAsync(sessionService.EmployeeId).ConfigureAwait(false);

            return Json(worklLoads);
        }

        public async Task<JsonResult> GetWorkloadForLeader()
        {
            IEnumerable<EmployeeWorkLoadModel> worklLoads = await employeeService.GetWorkloadForLeaderAsync(sessionService.EmployeeId).ConfigureAwait(false);

            return Json(worklLoads);
        }

        public async Task<JsonResult> GetWorkloadForPM()
        {
            IEnumerable<EmployeeWorkLoadModel> worklLoads = await employeeService.GetWorkloadForPMAsync(sessionService.EmployeeId).ConfigureAwait(false);

            return Json(worklLoads);
        }

        public async Task<JsonResult> GetNominationResult()
        {
            IEnumerable<EmployeeNominationModel> nominations = await employeeService.GetNominationResultAsync(sessionService.EmployeeId).ConfigureAwait(false);

            return Json(nominations);
        }
    }
}