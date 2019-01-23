namespace NexGoal.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Models.Common;
    using NexGoal.Models.Employee;
    using NexGoal.Models.EmployeeNomination;
    using NexGoal.Models.NominationPeriod;
    using NexGoal.Models.PARecord;
    using NexGoal.Models.Redmine;
    using NexGoal.Services;
    using NexGoal.Web.Core.Filters;
    using NexGoal.Web.Core.Services;

    [NexGoalPermission(FunctionKey = FunctionKey.MemberPage, Policy = Policy.Host)]
    public class MemberController : Controller
    {
        private readonly ICommonService commonService;
        private readonly ISessionService sessionService;
        private readonly IEmployeeService employeeService;
        private readonly IPARecordService paRecordService;
        private readonly INominationPeriodService nominationPeriodService;
        private readonly IEmployeeNominationService nominationService;
        private readonly IRedmineService redmineService;

        public MemberController(
            ISessionService sessionService,
            ICommonService commonService,
            IEmployeeService employeeService,
            IPARecordService paRecordService,
            INominationPeriodService nominationPeriodService,
            IEmployeeNominationService nominationService,
            IRedmineService redmineService)
        {
            this.sessionService = sessionService;
            this.commonService = commonService;
            this.employeeService = employeeService;
            this.paRecordService = paRecordService;
            this.nominationPeriodService = nominationPeriodService;
            this.nominationService = nominationService;
            this.redmineService = redmineService;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<EmployeeRoleModel> employeeRoles = await employeeService.GetRolesByEmployeeIdAsync(sessionService.EmployeeId).ConfigureAwait(false);
            bool isReviewer = employeeRoles.Any(x => x.RoleId == (int)SystemRole.Reviewer && x.HasRole);
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

            return View(member);
        }

        public async Task<JsonResult> GetMemberPerformanceAppraisalResult(int? periodId = null, int? memberId = null)
        {
            IEnumerable<PerformanceAppraisalResult> performanceAppraisals =
                await paRecordService.GetMemberPerformanceAppraisalResultAsync(sessionService.EmployeeId, periodId, memberId).ConfigureAwait(false);

            return Json(performanceAppraisals);
        }

        public async Task<JsonResult> GetNominationPeriod(bool? isCompleted = null)
        {
            IEnumerable<NominationPeriodModel> nominationPeriods = await nominationPeriodService.GetAsync(isCompleted).ConfigureAwait(false);

            return Json(nominationPeriods);
        }

        public async Task<JsonResult> GetMemberNominationResult(int? memberId = null, int? year = null)
        {
            IEnumerable<EmployeeNominationModel> nominations =
                await nominationService.GetMemberNominationResultAsync(sessionService.EmployeeId, memberId, year).ConfigureAwait(false);

            return Json(nominations);
        }

        public async Task<JsonResult> GetMembersWorkload(int periodId)
        {
            IEnumerable<TeamWorkloadModel> teamWorkloads = await redmineService.GetTeamWorkload(sessionService.EmployeeId, periodId).ConfigureAwait(false);

            return Json(teamWorkloads);
        }

        public async Task<JsonResult> GetRoleAccordingToMember(int memberId)
        {
            int roleId = await employeeService.GetRoleAccordingToMemberAsync(sessionService.EmployeeId, memberId).ConfigureAwait(false);

            return Json(roleId);
        }
    }
}