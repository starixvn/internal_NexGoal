namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using NexGoal.Models;
    using NexGoal.Models.Common;

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

        Task<IEnumerable<PeriodBaseModel>> GetAllPeriods();

        Task<PeriodBaseModel> GetCurrentPerformancePeriodAsync();

        Task<HRMSummaryModel> GetHRMSummary(int employeeId);

        Task<IEnumerable<string>> GetAnnouncements(int employeeId);

        Task<IEnumerable<PeriodBaseModel>> GetPeriodsForReportAsync();

        Task<IEnumerable<PerformanceGradeConversionModel>> GetPerformanceGradeConversionAsync();
    }
}