namespace NexGoal.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Models.PARecord;
    using NexGoal.Services;
    using NexGoal.Web.Core.Services;

    public class ReportController : Controller
    {
        private readonly ISessionService sessionService;
        private readonly IPARecordService paRecordService;

        public ReportController(
            ISessionService sessionService,
            IPARecordService paRecordService)
        {
            this.sessionService = sessionService;
            this.paRecordService = paRecordService;
        }

        public IActionResult PAReport()
        {
            return View();
        }

        public async Task<JsonResult> GetPARecords(GetPARecordMultiPeriodParam model)
        {
            model.ActorId = sessionService.EmployeeId;
            var paRecords = await paRecordService.GetPAReportAsync(model).ConfigureAwait(false);

            return Json(paRecords);
        }
    }
}