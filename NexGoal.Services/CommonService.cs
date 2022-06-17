namespace NexGoal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Caching.Memory;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Services;
    using NexGoal.Criteria;
    using NexGoal.Criteria.Common;
    using NexGoal.Criteria.Periods;
    using NexGoal.Criteria.Report;
    using NexGoal.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.PARecord;
    using NexGoal.Models.PerformanceRecord;
    using NexGoal.Models.Period;
    using NexGoal.Resources;

    public class CommonService : ICommonService
    {
        private readonly IBaseService baseService;
        private readonly IMemoryCache memoryCache;
        private readonly ICryptoService cryptoService;

        public CommonService(
            IBaseService baseService,
            IMemoryCache memoryCache,
            ICryptoService cryptoService)
        {
            this.baseService = baseService;
            this.memoryCache = memoryCache;
            this.cryptoService = cryptoService;
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

                refResources = await baseService.GetDataAsync<ReferenceResourceModel>(criteria);
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

            var employees = await baseService.GetDataAsync<Employee>(criteria);
            return employees;
        }

        public async Task<Employee> GetEmployeeByIdAsync(int employeeId)
        {
            var criteria = new SearchEmployeeCriteria
            {
                EmployeeId = employeeId
            };
            var employee = await baseService.GetDataEntityAsync<Employee>(criteria) ?? new Employee();

            return employee;
        }

        public async Task<IEnumerable<Group>> GetGroupsAsync()
        {
            var criteria = new GetGroupsCriteria();

            return await baseService.GetDataAsync<Group>(criteria);
        }

        public async Task<IEnumerable<ProjectModel>> GetProjectsAsync()
        {
            var criteria = new GetProjectsCriteria();
            var projects = await baseService.GetDataAsync<ProjectModel>(criteria);

            return projects.OrderBy(x => x.Name);
        }

        public async Task<IEnumerable<PerformanceGrade>> GetPerformanceGradeAsync()
        {
            memoryCache.TryGetValue(CacheKeys.PerformanceGrades, out IEnumerable<PerformanceGrade> performanceGrades);
            if (performanceGrades == null)
            {
                var criteria = new GetPerformanceGradeCriteria();
                performanceGrades = await baseService.GetDataAsync<PerformanceGrade>(criteria);

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

            return await baseService.GetDataAsync<PeriodBaseModel>(criteria);
        }

        public async Task<PeriodModel> GetCurrentPerformancePeriodAsync()
        {
            var criteria = new GetCurrentPerformancePeriodCriteria();

            return await baseService.GetDataEntityAsync<PeriodModel>(criteria);
        }

        public async Task<HRMSummaryModel> GetHRMSummaryAsync(int employeeId)
        {
            var criteria = new GetHRMSummaryCriteria
            {
                EmployeeId = employeeId
            };

            var hrm = await baseService.GetDataEntityAsync<HRMSummaryModel>(criteria);

            return hrm;
        }

        public async Task<IEnumerable<string>> GetAnnouncementAsync(int employeeId)
        {
            var criteria = new GetAnnouncementCriteria
            {
                EmployeeId = employeeId
            };

            return await baseService.GetDataAsync<string>(criteria);
        }

        public async Task<IEnumerable<PeriodBaseModel>> GetPeriodForReportAsync()
        {
            return await baseService.GetDataAsync<PeriodBaseModel>(new GetPeriodForReportCriteria());
        }

        public async Task<IEnumerable<ProjectModel>> GetProjectTagByEmployeeIdAsync(int employeeId)
        {
            var criteria = new GetProjectTagByEmployeeIdCriteria
            {
                EmployeeId = employeeId
            };
            var projects = await baseService.GetDataAsync<ProjectModel>(criteria);

            return projects.OrderBy(x => x.Name);
        }

        public void MappingGrade(IEnumerable<PARecordModel> performanceAppraisals, IEnumerable<PerformanceGrade> performanceGrades)
        {
            foreach (PARecordModel performanceAppraisal in performanceAppraisals)
            {
                MappingGrade(performanceAppraisal, performanceGrades);
            }
        }

        public void MappingGrade(PARecordModel performanceAppraisal, IEnumerable<PerformanceGrade> performanceGrades)
        {
            performanceAppraisal.GradeId = cryptoService.Decrypt(performanceAppraisal.GradeId);
            int.TryParse(performanceAppraisal.GradeId, out int gradeId);
            performanceAppraisal.Grade = performanceGrades.FirstOrDefault(x => x.GradeId == gradeId);

            performanceAppraisal.OriginalGradeId = cryptoService.Decrypt(performanceAppraisal.OriginalGradeId);
            int.TryParse(performanceAppraisal.OriginalGradeId, out gradeId);
            performanceAppraisal.OriginalGrade = performanceGrades.FirstOrDefault(x => x.GradeId == gradeId);

            MappingGrade(performanceAppraisal.Processes, performanceGrades);
        }

        public void MappingGrade(IEnumerable<PAProcessModel> processes, IEnumerable<PerformanceGrade> performanceGrades)
        {
            foreach (PAProcessModel process in processes)
            {
                foreach (PerformanceRecord performanceRecord in process.RelatedPerformanceRecords)
                {
                    performanceRecord.GradeId = cryptoService.Decrypt(performanceRecord.GradeId);
                    int.TryParse(performanceRecord.GradeId, out int gradeId);
                    performanceRecord.Grade = performanceGrades.FirstOrDefault(x => x.GradeId == gradeId);
                }

                MappingGrade(process, performanceGrades);
            }
        }

        public void MappingGrade(PAProcessModel process, IEnumerable<PerformanceGrade> performanceGrades)
        {
            process.GradeId = cryptoService.Decrypt(process.GradeId);
            int.TryParse(process.GradeId, out int gradeId);
            PerformanceGrade grade = performanceGrades.FirstOrDefault(x => x.GradeId == gradeId);

            if (grade != null)
            {
                process.Grade = grade.Name;

                if (process.ProcessId == (byte)PerformanceProcess.Evaluator && !process.RelatedPerformanceRecords.Any())
                {
                    process.RelatedPerformanceRecords = new List<PerformanceRecord>
                    {
                        new PerformanceRecord
                        {
                            Creator = process.Evaluator,
                            TypeId = (byte)PerformanceRecordType.PerformanceCheckin,
                            Grade = grade,
                            Comment = "NexGoal: This is the last period's grade, as no Performance Record was checked in this period."
                        }
                    };
                }

                return;
            }

            process.Grade = !string.IsNullOrWhiteSpace(process.RelatedPerformanceRecordIds) ? ViewLang.NA : string.Empty;
        }

        public string RemoveInvalidXmlChars(string content)
        {
            if (content != null)
            {
                return new string(content.Where(ch => System.Xml.XmlConvert.IsXmlChar(ch)).ToArray());
            }
            return content;
        }

    }
}