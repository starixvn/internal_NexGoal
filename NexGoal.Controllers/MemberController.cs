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
        private readonly ICommonService commonService;
        private readonly ISessionService sessionService;
        private readonly IEmployeeService employeeService;
        private readonly IEmployeeNominationService nominationService;

        public MemberController(
            ICommonService commonService,
            ISessionService sessionService,
            IEmployeeService employeeService,
            IEmployeeNominationService nominationService)
        {
            this.commonService = commonService;
            this.sessionService = sessionService;
            this.employeeService = employeeService;
            this.nominationService = nominationService;
        }

        public async Task<IActionResult> Index()
        {
            bool isNominator = await nominationService.IsNominator(sessionService.EmployeeId).ConfigureAwait(false);
            return View(isNominator);
        }

        public async Task<IActionResult> Details(int id)

        {
            IEnumerable<EmployeeBaseModel> members = await employeeService.GetMemberAsync(sessionService.EmployeeId).ConfigureAwait(false);

            if (!members.Any(x => x.EmployeeId == id))
            {
                return RedirectToAction("AccessDenied", "Error");
            }
            EmployeeBaseModel member = await commonService.GetEmployeeByIdAsync(id).ConfigureAwait(false);
            ViewData["IsNominator"] = await nominationService.IsNominator(sessionService.EmployeeId).ConfigureAwait(false);
            return View(member);
        }

        public async Task<JsonResult> GetMemberPerformanceAppraisalResult(int? periodId = null, int? memberId = null)
        {
            IEnumerable<PARecordModel> performanceAppraisals =
                await employeeService.GetMemberPerformanceAppraisalResultAsync(sessionService.EmployeeId, periodId, memberId).ConfigureAwait(false);

            return Json(performanceAppraisals);
        }

        public async Task<JsonResult> GetMemberNominationResult(int? memberId = null)
        {
            IEnumerable<EmployeeNominationModel> nominations = await employeeService.GetMemberNominationResultAsync(sessionService.EmployeeId, memberId).ConfigureAwait(false);

            return Json(nominations);
        }

        public async Task<JsonResult> GetMemberNominationDetails(
            int? nominationPeriodId = null,
            int? employeeId = null)
        {
            IEnumerable<EmployeeNominationModel> nominationDetails = await employeeService.GetMemberNominationDetailsAsync(
                sessionService.EmployeeId,
                nominationPeriodId,
                employeeId).ConfigureAwait(false);

            return Json(nominationDetails);
        }
    }
}