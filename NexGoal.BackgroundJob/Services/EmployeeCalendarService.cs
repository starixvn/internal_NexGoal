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

    public class EmployeeCalendarService(
        IBaseService baseService, 
        IHttpClientService httpClientService,
        SyncHrmSetting syncHrmApiSettings) : IEmployeeCalendarService
    {

        public async Task<IEnumerable<EmployeeCalendar>> GetCalendarsAsync(DateTime? lastModifiedDate = null)
        {
             var parameters = new Dictionary<string, object>{
                {"lastModifiedDate", lastModifiedDate?.ToString("yyyy-MM-ddTHH:mm:ssZ") ?? new DateTime(2025, 1, 1).ToString("yyyy-MM-ddTHH:mm:ssZ")}
            };

            var result = await httpClientService.ExecuteGetAsync<List<EmployeeCalendar>>(syncHrmApiSettings.Url + "/employee/calendar", syncHrmApiSettings.ApiKey, parameters);
            return result;
        }

         public async Task<DateTime?> GetLastModifiedDateAsync()
        {
            var lastModifiedDate = await baseService.GetDataEntityAsync<DateTime?>(new SyncEmployeeCalendarGetLastModifiedDateCriteria{});
            return lastModifiedDate;
        }

        public async Task<CommandResult> ImportCalendarsAsync(SyncEmployeeCalendarCriteria criteria)
        {
            return await baseService.ExecuteNonQueryAsync(criteria);
        }
    }
}