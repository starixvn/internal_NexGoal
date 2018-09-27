namespace NexGoal.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Models.PARecord;
    using NexGoal.Services;

    public class HomeController : Controller
    {
        private readonly IDashboardService dashboardService;

        public HomeController(IDashboardService dashboardService)
        {
            this.dashboardService = dashboardService;
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
    }
}