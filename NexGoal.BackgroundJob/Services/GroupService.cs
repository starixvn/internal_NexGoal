namespace NexGoal.BackgroundJob.Services
{
    using NexGoal.BackgroundJob.Criteria.Hrm;
    using NexGoal.BackgroundJob.Models.Hrm;
    using NexGoal.BackgroundJob.Services.Interfaces;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;

    public class GroupService(
        IBaseService baseService,
        IHttpClientService httpClientService,
        SyncHrmSetting syncHrmApiSettings) : IGroupService
    {
        public async Task<IEnumerable<Group>> GetGroupsAsync(DateTime? lastModifiedDate)
        {
            var defaultModifiedDate = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero).DateTime;
            var parameters = new Dictionary<string, object>
            {
                {"lastModifiedDate", lastModifiedDate?.ToString("yyyy-MM-ddTHH:mm:ssZ") ?? defaultModifiedDate.ToString("yyyy-MM-ddTHH:mm:ssZ")}
            };
            return await httpClientService.ExecuteGetAsync<List<Group>>(syncHrmApiSettings.Url + "/group", syncHrmApiSettings.ApiKey, parameters);
        }

        public async Task<DateTime?> GetLastModifiedDateAsync()
        {
            var criteria = new SyncGroupGetLastModifiedDateCriteria();
            return await baseService.GetDataEntityAsync<DateTime?>(criteria);
        }

        public async Task<CommandResult> ImportGroupsAsync(SyncGroupCriteria criteria)
        {
            return await baseService.ExecuteNonQueryAsync(criteria);
        }
    }
}