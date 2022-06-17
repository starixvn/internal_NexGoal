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

    public class CommonController : BaseController
    {
        private readonly ICommonService commonService;
        private readonly IEmployeeService employeeService;
        private readonly IDisciplineService disciplineService;

        public CommonController(
            ISessionService sessionService,
            ICommonService commonService,
            IEmployeeService employeeService,
            IDisciplineService disciplineService) : base(sessionService)
        {
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
            IEnumerable<Employee> employees = await commonService.GetEmployeesAsync(keyword, groupIds, isEnabled);

            return Json(employees);
        }

        [HttpGet]
        public async Task<JsonResult> GetGroups()
        {
            IEnumerable<Group> groups = await commonService.GetGroupsAsync();

            return Json(groups);
        }

        [HttpGet]
        public async Task<JsonResult> GetTargetTypes()
        {
            string[] refNames = { ReferenceResource.TargetType };
            IEnumerable<ReferenceResourceModel> referenceResources = await commonService.GetReferenceResourcesByNamesAsync(refNames);
            var targetTypes = referenceResources.Select(x => new TargetTypeModel { TargetTypeId = x.ResourceId, Name = x.Value });

            return Json(targetTypes);
        }

        [HttpGet]
        public async Task<JsonResult> GetEvaluateStatuses()
        {
            string[] refNames = { ReferenceResource.EvaluateStatus };
            var referenceResources = await commonService.GetReferenceResourcesByNamesAsync(refNames);
            var statuses = referenceResources.Select(x => new EvaluateStatusModel { Id = x.ResourceId, Name = x.Value });

            return Json(statuses);
        }

        [HttpGet]
        public async Task<JsonResult> GetPerformanceStatuses()
        {
            string[] refNames = { ReferenceResource.PerformanceStatus };
            var referenceResources = await commonService.GetReferenceResourcesByNamesAsync(refNames);
            IEnumerable<PerformanceStatusModel> statuses =
                referenceResources.Select(x => new PerformanceStatusModel { Id = x.ResourceId, Name = x.Value })
                .OrderBy(x => x.Id);

            return Json(statuses);
        }

        [HttpGet]
        public async Task<JsonResult> GetProjectTags()
        {
            IEnumerable<ProjectModel> projects = await commonService.GetProjectTagByEmployeeIdAsync(sessionService.EmployeeId);

            return Json(projects);
        }

        [HttpGet]
        public async Task<JsonResult> GetPerformanceGrades()
        {
            IEnumerable<PerformanceGrade> performanceGrades = await commonService.GetPerformanceGradeAsync();

            return Json(performanceGrades);
        }

        [HttpGet]
        public async Task<JsonResult> GetAllPeriods()
        {
            IEnumerable<PeriodBaseModel> periods = await commonService.GetAllPeriodAsync();

            return Json(periods);
        }

        [HttpGet]
        public async Task<JsonResult> GetCurrentPerformancePeriod()
        {
            PeriodModel period = await commonService.GetCurrentPerformancePeriodAsync();

            return Json(period);
        }

        [HttpGet]
        public async Task<JsonResult> GetMyMember(string keyword = null)
        {
            IEnumerable<Employee> members = await employeeService.GetMemberAsync(sessionService.EmployeeId, keyword);

            return Json(members);
        }

        [HttpGet]
        public async Task<JsonResult> GetMyGroup()
        {
            IEnumerable<Group> groups = await employeeService.GetGroupAsync(sessionService.EmployeeId);

            return Json(groups);
        }

        [HttpGet]
        public async Task<JsonResult> GetHRMSummary(int? memberId = null)
        {
            var employeeId = memberId ?? sessionService.EmployeeId;
            var HRMSummay = await commonService.GetHRMSummaryAsync(employeeId);

            return Json(HRMSummay);
        }

        [HttpGet]
        public async Task<JsonResult> GetDisciplineByEmployeeId(int? memberId = null)
        {
            var employeeId = memberId ?? sessionService.EmployeeId;
            IEnumerable<DisciplineModel> disciplines = await disciplineService.GetDisciplineByEmployeeIdAsync(employeeId);

            return Json(disciplines);
        }

        [HttpGet]
        public async Task<JsonResult> GetEOQQuestions()
        {
            string[] refNames = { ReferenceResource.EOQQuestion };
            IEnumerable<ReferenceResourceModel> referenceResources = await commonService.GetReferenceResourcesByNamesAsync(refNames);

            return Json(referenceResources.OrderBy(x => x.ResourceId));
        }

        [HttpGet]
        public async Task<JsonResult> GetEOYQuestions()
        {
            string[] refNames = { ReferenceResource.EOYQuestion };
            IEnumerable<ReferenceResourceModel> referenceResources = await commonService.GetReferenceResourcesByNamesAsync(refNames);

            return Json(referenceResources.OrderBy(x => x.ResourceId));
        }

        public async Task<JsonResult> GetSystemRoles()
        {
            string[] refNames = { ReferenceResource.SystemRole };
            IEnumerable<ReferenceResourceModel> roles = await commonService.GetReferenceResourcesByNamesAsync(refNames);

            return Json(roles.OrderBy(x => x.ResourceId));
        }

        [HttpGet]
        public async Task<JsonResult> GetAnnouncements()
        {
            IEnumerable<string> annoucements = await commonService.GetAnnouncementAsync(sessionService.EmployeeId);

            return Json(annoucements);
        }

        [HttpGet]
        public async Task<IActionResult> GetPeriodsForReport()
        {
            IEnumerable<PeriodBaseModel> periods = await commonService.GetPeriodForReportAsync();

            return Json(periods);
        }
    }
}