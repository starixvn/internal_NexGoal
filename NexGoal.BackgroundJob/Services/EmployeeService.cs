namespace NexGoal.BackgroundJob.Services
{
    using NexGoal.Core.Services;
    using NexGoal.BackgroundJob.Models.Hrm;
    using NexGoal.BackgroundJob.Services.Interfaces;
    using NexGoal.Core.Models;
    using NexGoal.BackgroundJob.Criteria.Hrm;

    public class EmployeeService(
        IBaseService baseService,
        IHttpClientService httpClientService,
        SyncHrmSetting syncHrmApiSettings) : IEmployeeService
    {
        public async Task<IEnumerable<Employee>> GetEmployeesAsync(DateTime? lastModifiedDate)
        {
            var defaultModifiedDate = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero).DateTime;
            var parameters = new Dictionary<string, object>
            {
                {"lastModifiedDate", lastModifiedDate?.ToString("yyyy-MM-ddTHH:mm:ssZ") ?? defaultModifiedDate.ToString("yyyy-MM-ddTHH:mm:ssZ")}
            };
            return await httpClientService.ExecuteGetAsync<List<Employee>>(syncHrmApiSettings.Url + "/employee", syncHrmApiSettings.ApiKey, parameters);
        }

        public async Task<DateTime?> GetLastModifiedDateAsync()
        {
            var criteria = new SyncEmployeeGetLastModifiedDateCriteria();
            return await baseService.GetDataEntityAsync<DateTime?>(criteria);
        }

        public async Task<CommandResult> ImportEmployeesAsync(SyncEmployeeCriteria criteria)
        {
            return await baseService.ExecuteNonQueryAsync(criteria);
        }
    }
}