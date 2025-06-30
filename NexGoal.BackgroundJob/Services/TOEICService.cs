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

    public class TOEICService(
        IBaseService baseService,
        IHttpClientService httpClientService,
        SyncHrmSetting syncHrmApiSettings) : ITOEICService
    {
        public async Task<IEnumerable<TOEIC>> GetTOEICsAsync(DateTime? lastModifiedDate = null)
        {
            var parameters = new Dictionary<string, object>{
                {"lastModifiedDate", lastModifiedDate?.ToString("yyyy-MM-ddTHH:mm:ssZ") ?? new DateTime(2000, 1, 1).ToString("yyyy-MM-ddTHH:mm:ssZ")}
            };

            var result = await httpClientService.ExecuteGetAsync<List<TOEIC>>(syncHrmApiSettings.Url + "/employee/toeic", syncHrmApiSettings.ApiKey, parameters);
            return result;
        }

        public async Task<DateTime?> GetLastModifiedDateAsync()
        {
            var lastModifiedDate = await baseService.GetDataEntityAsync<DateTime?>(new SyncTOEICGetLastModifiedDateCriteria{});
            return lastModifiedDate;
        }

        public async Task<CommandResult> ImportTOEICsAsync(SyncTOEICCriteria criteria)
        {
            return await baseService.ExecuteNonQueryAsync(criteria);
        }
    }
} 