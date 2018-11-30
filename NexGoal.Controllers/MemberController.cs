namespace NexGoal.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Filters;
    using NexGoal.Core.Services;
    using NexGoal.Models.Common;
    using NexGoal.Models.EmployeeNomination;
    using NexGoal.Models.NominationPeriod;
    using NexGoal.Models.PARecord;
    using NexGoal.Models.Redmine;
    using NexGoal.Services;

    [NexGoalPermission(FunctionKey = FunctionKey.MemberPage, Policy = Policy.Host)]
    public class MemberController : Controller
    {
        private readonly ICommonService commonService;
        private readonly ISessionService sessionService;
        private readonly IEmployeeService employeeService;
        private readonly INominationPeriodService nominationPeriodService;
        private readonly IEmployeeNominationService nominationService;
        private readonly IRedmineService redmineService;

        public MemberController(
            ISessionService sessionService,
            ICommonService commonService,
            IEmployeeService employeeService,
            INominationPeriodService nominationPeriodService,
            IEmployeeNominationService nominationService,
            IRedmineService redmineService)
        {
            this.sessionService = sessionService;
            this.commonService = commonService;
            this.employeeService = employeeService;
            this.nominationPeriodService = nominationPeriodService;
            this.nominationService = nominationService;
            this.redmineService = redmineService;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<EmployeeBaseModel> members = await employeeService.GetMemberByProcessIdAsync(sessionService.EmployeeId, PerformanceProcess.Reviewer).ConfigureAwait(false);
            bool isReviewer = members.Any();
            bool isNominator = await nominationService.IsNominator(sessionService.EmployeeId).ConfigureAwait(false);
            ViewData["IsNominator"] = isNominator;
            ViewData["IsReviewer"] = isReviewer;

            return View();
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

        public async Task<JsonResult> GetNominationPeriod(bool? isCompleted = null)
        {
            IEnumerable<NominationPeriodModel> nominationPeriods = await nominationPeriodService.GetAsync(isCompleted).ConfigureAwait(false);

            return Json(nominationPeriods);
        }

        public async Task<JsonResult> GetMemberNominationResult(int? memberId = null)
        {
            IEnumerable<EmployeeNominationModel> nominations = await employeeService.GetMemberNominationResultAsync(sessionService.EmployeeId, memberId).ConfigureAwait(false);

            return Json(nominations);
        }

        [HttpGet]
        public async Task<JsonResult> GetMembersWorkload(int periodId)
        {
            IEnumerable<TeamWorkloadModel> teamWorkloads = await redmineService.GetTeamWorkload(sessionService.EmployeeId, periodId).ConfigureAwait(false);

            return Json(teamWorkloads);
        }
    }
}