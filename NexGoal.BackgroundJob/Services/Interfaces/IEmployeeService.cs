namespace NexGoal.BackgroundJob.Services
{
    using System.Threading.Tasks;
    using NexGoal.BackgroundJob.Criteria.Hrm;
    using NexGoal.BackgroundJob.Models.Hrm;
    using NexGoal.Core.Models;

    public interface IEmployeeService
    {
        Task<IEnumerable<Employee>> GetEmployeesAsync(DateTime? lastModifiedDate);

        Task<CommandResult> ImportEmployeesAsync(SyncEmployeeCriteria criteria);

        Task<DateTime?> GetLastModifiedDateAsync();

    }
}