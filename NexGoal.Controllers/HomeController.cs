namespace NexGoal.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Models.PARecord;
    using NexGoal.Core.Services;
    using NexGoal.Services;

    public class HomeController : Controller
    {
        private readonly ISessionService sessionService;
        private readonly IDashboardService dashboardService;

        public HomeController(IDashboardService dashboardService, ISessionService sessionService)
        {
            this.dashboardService = dashboardService;
            this.sessionService = sessionService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult WorkLoad()
        {
            return View();
        }

        public async Task<JsonResult> GetPARecords()
        {
            IEnumerable<PARecordModel> records = await dashboardService.GetPARecordAsync().ConfigureAwait(false);

            return Json(records);
        }

        public async Task<IActionResult> GetWorkLoad()
        {
            var worklLoad = await dashboardService.GetWorkLoad(sessionService.EmployeeId).ConfigureAwait(false);

            return Json(worklLoad);
        }
    }
}