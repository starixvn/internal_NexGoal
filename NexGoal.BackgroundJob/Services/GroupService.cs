namespace NexGoal.BackgroundJob.Services
{
    using NexGoal.Core.Services;
    using NexGoal.BackgroundJob.Models.Hrm;
    using NexGoal.BackgroundJob.Services.Interface;
    using NexGoal.Core.Models;
    using NexGoal.BackgroundJob.Criteria.Hrm;

    public class GroupService(
        IBaseService baseService, 
        IHttpClientService httpClientService,
        SyncHrmSetting syncHrmApiSettings) : IGroupService
    {

        public async Task<IEnumerable<Group>> GetGroupsAsync(DateTime? lastModifiedDate)
        {
            var parameters = new Dictionary<string, object>{
                {"lastModifiedDate", lastModifiedDate?.ToString("yyyy-MM-ddTHH:mm:ssZ") ?? new DateTime(2020, 1, 1).ToString("yyyy-MM-ddTHH:mm:ssZ")}
            };
            var groups = await httpClientService.ExecuteGetAsync<List<Group>>(syncHrmApiSettings.Url + "/group", syncHrmApiSettings.ApiKey, parameters);

            return groups;
        }

        public async Task<DateTime?> GetLastModifiedDateAsync()
        {
            var lastModifiedDate = await baseService.GetDataEntityAsync<DateTime?>(new SyncGroupGetLastModifiedDateCriteria{});
            return lastModifiedDate;
        }

        public async Task<CommandResult> ImportGroupsAsync(SyncGroupCriteria criteria)
        {
            return await baseService.ExecuteNonQueryAsync(criteria);
        }


    }
}