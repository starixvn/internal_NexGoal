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
            try
            {
                var parameters = new Dictionary<string, object>{
                    {"lastModifiedDate", lastModifiedDate?.ToString("yyyy-MM-ddTHH:mm:ssZ") ?? new DateTime(2020, 1, 1).ToString("yyyy-MM-ddTHH:mm:ssZ")}
                };

                var result = await httpClientService.ExecuteGetAsync<object>(syncHrmApiSettings.Url + "/jobtitle", syncHrmApiSettings.ApiKey, parameters);
                if (result == null)
                {
                    return new List<JobTitle>();
                }

                var jsonString = JsonConvert.SerializeObject(result);
                return JsonConvert.DeserializeObject<List<JobTitle>>(jsonString) ?? new List<JobTitle>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetJobTitlesAsync: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return new List<JobTitle>();
            }
        }

        public async Task<CommandResult> ImportJobTitlesAsync(SyncJobTitleCriteria criteria)
        {
            return await baseService.ExecuteNonQueryAsync(criteria);
        }
    }
} 