namespace NexGoal.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.Period;
    using NexGoal.Services;

    public class CommonController : Controller
    {
        private readonly ICommonService commonService;

        public CommonController(ICommonService commonService)
        {
            this.commonService = commonService;
        }

        [HttpGet]
        public async Task<JsonResult> SearchEmployees(
            string keyword = null,
            int? groupId = null,
            bool? isEnabled = true)
        {
            IEnumerable<EmployeeBaseModel> employees = await commonService.GetEmployeesAsync(keyword, groupId, isEnabled).ConfigureAwait(false);

            return Json(employees);
        }

        [HttpGet]
        public async Task<JsonResult> GetGroups()
        {
            IEnumerable<GroupBaseModel> groups = await commonService.GetGroupsAsync().ConfigureAwait(false);

            return Json(groups);
        }

        [HttpGet]
        public async Task<JsonResult> GetTargetTypes()
        {
            string[] refNames = { ReferenceResource.TargetType };
            IEnumerable<ReferenceResourceModel> referenceResources = await commonService.GetReferenceResourcesByNamesAsync(refNames).ConfigureAwait(false);
            IEnumerable<TargetTypeModel> targetTypes = referenceResources.Select(x => new TargetTypeModel { TargetTypeId = x.ResourceId, Name = x.Value });

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
            IEnumerable<ProjectModel> projects = await commonService.GetProjectsAsync().ConfigureAwait(false);

            return Json(projects);
        }

        [HttpGet]
        public async Task<JsonResult> GetPerformanceGrades()
        {
            IEnumerable<PerformanceGradeModel> performanceGrades = await commonService.GetPerformanceGradeAsync().ConfigureAwait(false);

            return Json(performanceGrades);
        }

        [HttpGet]
        public async Task<JsonResult> GetAllPeriods()
        {
            var periods = await commonService.GetAllPeriods().ConfigureAwait(false);

            return Json(periods);
        }
    }
}