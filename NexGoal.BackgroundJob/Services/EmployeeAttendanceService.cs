namespace NexGoal.BackgroundJob.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.BackgroundJob.Criteria.Hrm;
    using NexGoal.BackgroundJob.Models.Hrm;
    using NexGoal.BackgroundJob.Services.Interface;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;

    public class EmployeeAttendanceService(
        IBaseService baseService, 
        IHttpClientService httpClientService,
        SyncHrmSetting syncHrmApiSettings) : IEmployeeAttendanceService
    {

        public async Task<IEnumerable<EmployeeAttendance>> GetAttendancesAsync(DateTime? lastModifiedDate = null)
        {
             var parameters = new Dictionary<string, object>{
                {"lastModifiedDate", lastModifiedDate?.ToString("yyyy-MM-ddTHH:mm:ssZ") ?? new DateTime(2025, 1, 1).ToString("yyyy-MM-ddTHH:mm:ssZ")}
            };

            var result = await httpClientService.ExecuteGetAsync<List<EmployeeAttendance>>(syncHrmApiSettings.Url + "/employee/attendance", syncHrmApiSettings.ApiKey, parameters);
            return result;
        }

         public async Task<DateTime?> GetLastModifiedDateAsync()
        {
            var lastModifiedDate = await baseService.GetDataEntityAsync<DateTime?>(new SyncEmployeeAttendanceGetLastModifiedDateCriteria{});
            return lastModifiedDate;
        }

        public async Task<CommandResult> ImportAttendancesAsync(SyncEmployeeAttendanceCriteria criteria)
        {
            return await baseService.ExecuteNonQueryAsync(criteria);
        }
    }
} 