namespace NexGoal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Caching.Memory;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Services;
    using NexGoal.Criteria;
    using NexGoal.Criteria.Common;
    using NexGoal.Criteria.Periods;
    using NexGoal.Criteria.Report;
    using NexGoal.Models;
    using NexGoal.Models.Common;

    public class CommonService : ICommonService
    {
        private readonly IBaseService baseService;
        private readonly IMemoryCache memoryCache;

        public CommonService(
            IBaseService baseService,
            IMemoryCache memoryCache)
        {
            this.baseService = baseService;
            this.memoryCache = memoryCache;
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

        public async Task<IEnumerable<Employee>> GetEmployeesAsync(
            string keyword = null,
            string groupIds = null,
            bool? isEnabled = true)
        {
            var criteria = new SearchEmployeeCriteria
            {
                Keyword = keyword,
                GroupIds = groupIds,
                IsEnabled = isEnabled
            };

            var employees = await baseService.GetDataAsync<Employee>(criteria).ConfigureAwait(false);
            return employees;
        }

        public async Task<Employee> GetEmployeeByIdAsync(int employeeId)
        {
            var criteria = new SearchEmployeeCriteria
            {
                EmployeeId = employeeId
            };
            var employee = await baseService.GetDataEntityAsync<Employee>(criteria).ConfigureAwait(false) ?? new Employee();

            return employee;
        }

        public async Task<IEnumerable<Group>> GetGroupsAsync()
        {
            var criteria = new GetGroupsCriteria();

            return await baseService.GetDataAsync<Group>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<ProjectModel>> GetProjectsAsync()
        {
            var criteria = new GetProjectsCriteria();
            var projects = await baseService.GetDataAsync<ProjectModel>(criteria).ConfigureAwait(false);

            return projects.OrderBy(x => x.Name);
        }

        public async Task<IEnumerable<PerformanceGrade>> GetPerformanceGradeAsync()
        {
            memoryCache.TryGetValue(CacheKeys.PerformanceGrades, out IEnumerable<PerformanceGrade> performanceGrades);
            if (performanceGrades == null)
            {
                var criteria = new GetPerformanceGradeCriteria();
                performanceGrades = await baseService.GetDataAsync<PerformanceGrade>(criteria).ConfigureAwait(false);

                var cacheExpirationOptions = new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromDays(30),
                    Priority = CacheItemPriority.Normal
                };
                memoryCache.Set(CacheKeys.PerformanceGrades, performanceGrades, cacheExpirationOptions);
            }

            return performanceGrades;
        }

        public async Task<IEnumerable<PeriodBaseModel>> GetAllPeriodAsync()
        {
            var criteria = new GetAllPeriodsCriteria();

            return await baseService.GetDataAsync<PeriodBaseModel>(criteria).ConfigureAwait(false);
        }

        public async Task<PeriodBaseModel> GetCurrentPerformancePeriodAsync()
        {
            var criteria = new GetCurrentPerformancePeriodCriteria();

            return await baseService.GetDataEntityAsync<PeriodBaseModel>(criteria).ConfigureAwait(false);
        }

        public async Task<HRMSummaryModel> GetHRMSummaryAsync(int employeeId)
        {
            var criteria = new GetHRMSummaryCriteria
            {
                EmployeeId = employeeId
            };

            var hrm = await baseService.GetDataEntityAsync<HRMSummaryModel>(criteria).ConfigureAwait(false);

            return hrm;
        }

        public async Task<IEnumerable<string>> GetAnnouncementAsync(int employeeId)
        {
            var criteria = new GetAnnouncementCriteria
            {
                EmployeeId = employeeId
            };

            return await baseService.GetDataAsync<string>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<PeriodBaseModel>> GetPeriodForReportAsync()
        {
            return await baseService.GetDataAsync<PeriodBaseModel>(new GetPeriodForReportCriteria()).ConfigureAwait(false);
        }

        public async Task<IEnumerable<ProjectModel>> GetProjectByEmployeeIdAsync(int employeeId)
        {
            var criteria = new GetProjectByEmployeeIdCriteria
            {
                EmployeeId = employeeId
            };
            var projects = await baseService.GetDataAsync<ProjectModel>(criteria).ConfigureAwait(false);

            return projects.OrderBy(x => x.Name);
        }
    }
}