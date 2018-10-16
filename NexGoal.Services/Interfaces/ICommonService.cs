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

        Task<IEnumerable<EmployeeBaseModel>> GetEmployeesAsync(
            string keyword = null,
            int? groupId = null,
            bool? isEnabled = true);

        Task<EmployeeBaseModel> GetEmployeeByIdAsync(int employeeId);

        Task<IEnumerable<GroupBaseModel>> GetGroupsAsync();

        Task<IEnumerable<ProjectModel>> GetProjectsAsync();

        Task<IEnumerable<PerformanceGradeModel>> GetPerformanceGradeAsync();

        Task<IEnumerable<PeriodBaseModel>> GetAllPeriods();
    }
}