namespace NexGoal.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.Discipline;
    using NexGoal.Models.Period;
    using NexGoal.Services;
    using NexGoal.Web.Core.Services;

    public class CommonController : Controller
    {
        private readonly ISessionService sessionService;
        private readonly ICommonService commonService;
        private readonly IEmployeeService employeeService;
        private readonly IDisciplineService disciplineService;

        public CommonController(
            ISessionService sessionService,
            ICommonService commonService,
            IEmployeeService employeeService,
            IDisciplineService disciplineService)
        {
            this.sessionService = sessionService;
            this.commonService = commonService;
            this.employeeService = employeeService;
            this.disciplineService = disciplineService;
        }

        [HttpGet]
        public async Task<JsonResult> GetEmployees(
            string keyword = null,
            string groupIds = null,
            bool? isEnabled = true)
        {
            IEnumerable<Employee> employees = await commonService.GetEmployeesAsync(keyword, groupIds, isEnabled).ConfigureAwait(false);

            return Json(employees);
        }

        [HttpGet]
        public async Task<JsonResult> GetGroups()
        {
            IEnumerable<Group> groups = await commonService.GetGroupsAsync().ConfigureAwait(false);

            return Json(groups);
        }

        [HttpGet]
        public async Task<JsonResult> GetTargetTypes()
        {
            string[] refNames = { ReferenceResource.TargetType };
            IEnumerable<ReferenceResourceModel> referenceResources = await commonService.GetReferenceResourcesByNamesAsync(refNames).ConfigureAwait(false);
            var targetTypes = referenceResources.Select(x => new TargetTypeModel { TargetTypeId = x.ResourceId, Name = x.Value });

            return Json(targetTypes);
        }

        [HttpGet]
        public async Task<JsonResult> GetEvaluateStatuses()
        {
            string[] refNames = { ReferenceResource.EvaluateStatus };
            var referenceResources = await commonService.GetReferenceResourcesByNamesAsync(refNames).ConfigureAwait(false);
            var statuses = referenceResources.Select(x => new EvaluateStatusModel { Id = x.ResourceId, Name = x.Value });

            return Json(statuses);
        }

        [HttpGet]
        public async Task<JsonResult> GetPerformanceStatuses()
        {
            string[] refNames = { ReferenceResource.PerformanceStatus };
            var referenceResources = await commonService.GetReferenceResourcesByNamesAsync(refNames).ConfigureAwait(false);
            IEnumerable<PerformanceStatusModel> statuses =
                referenceResources.Select(x => new PerformanceStatusModel { Id = x.ResourceId, Name = x.Value })
                .OrderBy(x => x.Id);

            return Json(statuses);
        }

        [HttpGet]
        public async Task<JsonResult> GetProjects()
        {
            IEnumerable<ProjectModel> projects = await commonService.GetProjectByEmployeeIdAsync(sessionService.EmployeeId).ConfigureAwait(false);

            return Json(projects);
        }

        [HttpGet]
        public async Task<JsonResult> GetPerformanceGrades()
        {
            IEnumerable<PerformanceGrade> performanceGrades = await commonService.GetPerformanceGradeAsync().ConfigureAwait(false);

            return Json(performanceGrades);
        }

        [HttpGet]
        public async Task<JsonResult> GetAllPeriods()
        {
            IEnumerable<PeriodBaseModel> periods = await commonService.GetAllPeriods().ConfigureAwait(false);

            return Json(periods);
        }

        [HttpGet]
        public async Task<JsonResult> GetCurrentPerformancePeriod()
        {
            PeriodBaseModel period = await commonService.GetCurrentPerformancePeriodAsync().ConfigureAwait(false);

            return Json(period);
        }

        [HttpGet]
        public async Task<JsonResult> GetMyMember(string keyword = null)
        {
            IEnumerable<Employee> members = await employeeService.GetMemberAsync(sessionService.EmployeeId, keyword).ConfigureAwait(false);

            return Json(members);
        }

        [HttpGet]
        public async Task<JsonResult> GetMyGroup()
        {
            IEnumerable<Group> groups = await employeeService.GetGroupAsync(sessionService.EmployeeId).ConfigureAwait(false);

            return Json(groups);
        }

        [HttpGet]
        public async Task<JsonResult> GetHRMSummary(int? memberId = null)
        {
            var employeeId = memberId ?? sessionService.EmployeeId;
            var HRMSummay = await commonService.GetHRMSummary(employeeId).ConfigureAwait(false);

            return Json(HRMSummay);
        }

        [HttpGet]
        public async Task<JsonResult> GetDisciplineByEmployeeId(int? memberId = null)
        {
            var employeeId = memberId ?? sessionService.EmployeeId;
            IEnumerable<DisciplineModel> disciplines = await disciplineService.GetDisciplineByEmployeeIdAsync(employeeId).ConfigureAwait(false);

            return Json(disciplines);
        }

        [HttpGet]
        public async Task<JsonResult> GetEOQQuestions()
        {
            string[] refNames = { ReferenceResource.EOQQuestion };
            IEnumerable<ReferenceResourceModel> referenceResources = await commonService.GetReferenceResourcesByNamesAsync(refNames).ConfigureAwait(false);

            return Json(referenceResources.OrderBy(x => x.ResourceId));
        }

        [HttpGet]
        public async Task<JsonResult> GetEOYQuestions()
        {
            string[] refNames = { ReferenceResource.EOYQuestion };
            IEnumerable<ReferenceResourceModel> referenceResources = await commonService.GetReferenceResourcesByNamesAsync(refNames).ConfigureAwait(false);

            return Json(referenceResources.OrderBy(x => x.ResourceId));
        }

        public async Task<JsonResult> GetSystemRoles()
        {
            string[] refNames = { ReferenceResource.SystemRole };
            IEnumerable<ReferenceResourceModel> roles = await commonService.GetReferenceResourcesByNamesAsync(refNames).ConfigureAwait(false);

            return Json(roles.OrderBy(x => x.ResourceId));
        }

        [HttpGet]
        public async Task<JsonResult> GetAnnouncements()
        {
            IEnumerable<string> annoucements = await commonService.GetAnnouncementAsync(sessionService.EmployeeId).ConfigureAwait(false);

            return Json(annoucements);
        }

        [HttpGet]
        public async Task<IActionResult> GetPeriodsForReport()
        {
            IEnumerable<PeriodBaseModel> periods = await commonService.GetPeriodForReportAsync().ConfigureAwait(false);

            return Json(periods);
        }
    }
}