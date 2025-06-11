namespace NexGoal.BackgroundJob.Services
{
    using NexGoal.Core.Services;
    using NexGoal.BackgroundJob.Models.Hrm;
    using NexGoal.BackgroundJob.Services.Interface;
    using NexGoal.Core.Models;
    using NexGoal.BackgroundJob.Criteria.Hrm;

    public class EmployeeService(
        IBaseService baseService, 
        IHttpClientService httpClientService,
        SyncHrmSetting syncHrmApiSettings) : IEmployeeService
    {

        public async Task<IEnumerable<Employee>> GetEmployeesAsync(DateTime? lastModifiedDate)
        {
            var parameters = new Dictionary<string, object>{
                {"lastModifiedDate", lastModifiedDate?.ToString("yyyy-MM-ddTHH:mm:ssZ") ?? new DateTime(2020, 1, 1).ToString("yyyy-MM-ddTHH:mm:ssZ")}
            };
            var employees = await httpClientService.ExecuteGetAsync<List<Employee>>(syncHrmApiSettings.Url + "/employee", syncHrmApiSettings.ApiKey, parameters);

            return employees;
        }

        public async Task<DateTime?> GetLastModifiedDateAsync()
        {
            var lastModifiedDate = await baseService.GetDataEntityAsync<DateTime?>(new SyncEmployeeGetLastModifiedDateCriteria{});
            return lastModifiedDate;
        }

        public async Task<CommandResult> ImportEmployeesAsync(SyncEmployeeCriteria criteria)
        {
            return await baseService.ExecuteNonQueryAsync(criteria);
        }


    }
}