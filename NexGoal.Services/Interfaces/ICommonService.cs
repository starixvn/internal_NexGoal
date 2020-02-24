namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using NexGoal.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.PARecord;
    using NexGoal.Models.Period;

    public interface ICommonService
    {
        Task<IEnumerable<ReferenceResourceModel>> GetReferenceResourcesByNamesAsync(string[] refNames, bool? isActive = true);

        SelectList GetSelectList<T>(IEnumerable<T> items, string dataValueField, string dataTextField);

        Task<IEnumerable<Employee>> GetEmployeesAsync(
            string keyword = null,
            string groupIds = null,
            bool? isEnabled = true);

        Task<Employee> GetEmployeeByIdAsync(int employeeId);

        Task<IEnumerable<Group>> GetGroupsAsync();

        Task<IEnumerable<ProjectModel>> GetProjectsAsync();

        Task<IEnumerable<PerformanceGrade>> GetPerformanceGradeAsync();

        Task<IEnumerable<PeriodBaseModel>> GetAllPeriodAsync();

        Task<PeriodModel> GetCurrentPerformancePeriodAsync();

        Task<HRMSummaryModel> GetHRMSummaryAsync(int employeeId);

        Task<IEnumerable<string>> GetAnnouncementAsync(int employeeId);

        Task<IEnumerable<PeriodBaseModel>> GetPeriodForReportAsync();

        Task<IEnumerable<ProjectModel>> GetProjectTagByEmployeeIdAsync(int employeeId);

        void MappingGrade(IEnumerable<PARecordModel> performanceAppraisals, IEnumerable<PerformanceGrade> performanceGrades);

        void MappingGrade(PARecordModel performanceAppraisal, IEnumerable<PerformanceGrade> performanceGrades);

        void MappingGrade(IEnumerable<PAProcessModel> processes, IEnumerable<PerformanceGrade> performanceGrades);

        void MappingGrade(PAProcessModel process, IEnumerable<PerformanceGrade> performanceGrades);
    }
}