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
    using Newtonsoft.Json;

    public class JobTitleService(
        IBaseService baseService,
        IHttpClientService httpClientService,
        SyncHrmSetting syncHrmApiSettings) : IJobTitleService
    {
        public async Task<IEnumerable<JobTitle>> GetJobTitlesAsync(DateTime? lastModifiedDate = null)
        {
            var parameters = new Dictionary<string, object>{
                {"lastModifiedDate", lastModifiedDate?.ToString("yyyy-MM-ddTHH:mm:ssZ") ?? new DateTime(2020, 1, 1).ToString("yyyy-MM-ddTHH:mm:ssZ")}
            };

            return await httpClientService.ExecuteGetAsync<List<JobTitle>>(syncHrmApiSettings.Url + "/jobtitle", syncHrmApiSettings.ApiKey, parameters);

        }

        public async Task<CommandResult> ImportJobTitlesAsync(SyncJobTitleCriteria criteria)
        {
            return await baseService.ExecuteNonQueryAsync(criteria);
        }
    }
} 