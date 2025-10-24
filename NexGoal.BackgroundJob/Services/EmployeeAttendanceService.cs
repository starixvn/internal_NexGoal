namespace NexGoal.BackgroundJob.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.BackgroundJob.Criteria.Hrm;
    using NexGoal.BackgroundJob.Models.Hrm;
    using NexGoal.BackgroundJob.Services.Interfaces;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;

    public class EmployeeAttendanceService(
        IBaseService baseService,
        IHttpClientService httpClientService,
        SyncHrmSetting syncHrmApiSettings) : IEmployeeAttendanceService
    {
        public async Task<IEnumerable<EmployeeAttendance>> GetAttendancesAsync(DateTime? lastModifiedDate)
        {
            var defaultModifiedDate = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero).DateTime;
            var parameters = new Dictionary<string, object>
            {
                {"lastModifiedDate", lastModifiedDate?.ToString("yyyy-MM-ddTHH:mm:ssZ") ?? defaultModifiedDate.ToString("yyyy-MM-ddTHH:mm:ssZ")}
            };

            return await httpClientService.ExecuteGetAsync<List<EmployeeAttendance>>(syncHrmApiSettings.Url + "/employee/attendance", syncHrmApiSettings.ApiKey, parameters);
        }

        public async Task<DateTime?> GetLastModifiedDateAsync()
        {
            var criteria = new SyncEmployeeAttendanceGetLastModifiedDateCriteria();
            return await baseService.GetDataEntityAsync<DateTime?>(criteria);
        }

        public async Task<CommandResult> ImportAttendancesAsync(SyncEmployeeAttendanceCriteria criteria)
        {
            return await baseService.ExecuteNonQueryAsync(criteria);
        }
    }
}