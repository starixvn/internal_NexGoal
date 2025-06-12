namespace NexGoal.BackgroundJob.Services
{
    using System.Threading.Tasks;
    using NexGoal.BackgroundJob.Criteria.Hrm;
    using NexGoal.BackgroundJob.Models.Hrm;
    using NexGoal.Core.Models;

    public interface IEmployeeCalendarService
    {
        Task<IEnumerable<EmployeeCalendar>> GetCalendarsAsync(DateTime? lastModifiedDate);

        Task<CommandResult> ImportCalendarsAsync(SyncEmployeeCalendarCriteria criteria);

        Task<DateTime?> GetLastModifiedDateAsync();

    }
}