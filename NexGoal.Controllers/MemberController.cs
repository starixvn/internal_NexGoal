namespace NexGoal.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Services;
    using NexGoal.Models.Common;
    using NexGoal.Models.EmployeeNomination;
    using NexGoal.Models.PARecord;
    using NexGoal.Services;

    public class MemberController : Controller
    {
        private readonly ISessionService sessionService;
        private readonly IEmployeeService employeeService;

        public MemberController(
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

        public async Task<IActionResult> Details(int id)
        {
            IEnumerable<EmployeeBaseModel> members = await employeeService.GetMemberAsync(sessionService.EmployeeId).ConfigureAwait(false);

            if (!members.Any(x => x.EmployeeId == id))
            {
                return RedirectToAction("AccessDenied", "Error");
            }

            return View(id);
        }

        public async Task<IActionResult> GetMemberPerformanceAppraisalResult(int? periodId = null, int? memberId = null)
        {
            IEnumerable<PARecordModel> performanceAppraisals =
                await employeeService.GetMemberPerformanceAppraisalResultAsync(sessionService.EmployeeId, periodId, memberId).ConfigureAwait(false);

            return Json(performanceAppraisals);
        }

        public async Task<IActionResult> GetMemberNominationResult(int? memberId = null)
        {
            IEnumerable<EmployeeNominationModel> nominations = await employeeService.GetMemberNominationResultAsync(sessionService.EmployeeId, memberId).ConfigureAwait(false);

            return Json(nominations);
        }
    }
}