namespace NexGoal.BackgroundJob.Services
{
    using System.Threading.Tasks;
    using NexGoal.BackgroundJob.Criteria.Hrm;
    using NexGoal.BackgroundJob.Models.Hrm;
    using NexGoal.Core.Models;

    public interface IEmployeeAttendanceService
    {
        Task<IEnumerable<EmployeeAttendance>> GetAttendancesAsync(DateTime? lastModifiedDate);

        Task<CommandResult> ImportAttendancesAsync(SyncEmployeeAttendanceCriteria criteria);

        Task<DateTime?> GetLastModifiedDateAsync();

    }
}