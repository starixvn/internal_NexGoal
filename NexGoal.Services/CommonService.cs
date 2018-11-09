namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Services;
    using NexGoal.Criteria;
    using NexGoal.Criteria.Common;
    using NexGoal.Criteria.Periods;
    using NexGoal.Models;
    using NexGoal.Models.Common;

    public class CommonService : ICommonService
    {
        private readonly IBaseService baseService;
        private readonly IAvatarService avatarService;

        public CommonService(
            IBaseService baseService,
            IAvatarService avatarService)
        {
            this.baseService = baseService;
            this.avatarService = avatarService;
        }

        public async Task<IEnumerable<ReferenceResourceModel>> GetReferenceResourcesByNamesAsync(string[] refNames, bool? isActive = true)
        {
            IEnumerable<ReferenceResourceModel> refResources = new List<ReferenceResourceModel>();

            if (refNames != null)
            {
                var criteria = new GetRefResourceByNamesCriteria
                {
                    RefNames = string.Join(",", refNames),
                    IsActive = isActive
                };

                refResources = await baseService.GetDataAsync<ReferenceResourceModel>(criteria).ConfigureAwait(false);
            }

            return refResources;
        }

        public SelectList GetSelectList<T>(IEnumerable<T> items, string dataValueField, string dataTextField)
        {
            return new SelectList(items, dataValueField, dataTextField);
        }

        public async Task<IEnumerable<EmployeeBaseModel>> GetEmployeesAsync(
            string keyword = null,
            int? groupId = null,
            bool? isEnabled = true)
        {
            var criteria = new SearchEmployeeCriteria
            {
                Keyword = keyword,
                GroupId = groupId,
                IsEnabled = isEnabled
            };

            var employees = await baseService.GetDataAsync<EmployeeBaseModel>(criteria).ConfigureAwait(false);
            foreach(EmployeeBaseModel employee in employees)
            {
                employee.Avatar = avatarService.GetFilePath(employee.Code);
            }

            return employees;
        }

        public async Task<EmployeeBaseModel> GetEmployeeByIdAsync(int employeeId)
        {
            var criteria = new SearchEmployeeCriteria
            {
                EmployeeId = employeeId
            };
            var employee = await baseService.GetDataEntityAsync<EmployeeBaseModel>(criteria).ConfigureAwait(false) ?? new EmployeeBaseModel();
            employee.Avatar = avatarService.GetFilePath(employee.Code);

            return employee;
        }

        public async Task<IEnumerable<GroupBaseModel>> GetGroupsAsync()
        {
            var criteria = new GetGroupsCriteria();

            return await baseService.GetDataAsync<GroupBaseModel>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<ProjectModel>> GetProjectsAsync()
        {
            var criteria = new GetProjectsCriteria();

            return await baseService.GetDataAsync<ProjectModel>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<PerformanceGradeModel>> GetPerformanceGradeAsync()
        {
            string[] refNames = { ReferenceResource.PerformanceGrade };
            IEnumerable<ReferenceResourceModel> referenceResources = await GetReferenceResourcesByNamesAsync(refNames).ConfigureAwait(false);
            IEnumerable<PerformanceGradeModel> performanceGrades = referenceResources.Select(x => new PerformanceGradeModel
            {
                GradeId = x.ResourceId,
                Name = x.Value,
                Description = x.Description
            }).OrderBy(x => x.GradeId);

            return performanceGrades;
        }

        public async Task<IEnumerable<PeriodBaseModel>> GetAllPeriods()
        {
            var criteria = new GetAllPeriodsCriteria();

            return await baseService.GetDataAsync<PeriodBaseModel>(criteria).ConfigureAwait(false);
        }

        public async Task<PeriodBaseModel> GetCurrentPerformancePeriodAsync()
        {
            var criteria = new GetCurrentPerformancePeriodCriteria();

            return await baseService.GetDataEntityAsync<PeriodBaseModel>(criteria).ConfigureAwait(false);
        }

        public async Task<HRMSummaryModel> GetHRMSummary(int empId)
        {
            var criteria = new GetHRMSummaryCriteria
            {
                EmployeeId = empId
            };

            return await baseService.GetDataEntityAsync<HRMSummaryModel>(criteria).ConfigureAwait(false);
        }
    }
}