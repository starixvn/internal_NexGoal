namespace NexGoal.BackgroundJob.Workers
{
    using Criteria.Redmine;
    using Models.Redmine;
    using NexGoal.Core.Services;

    public class SyncRedmineDataWorker : BaseWorker
    {
        private readonly IBaseService baseService;
        private readonly ILogger<SyncRedmineDataWorker> logfile;

        public SyncRedmineDataWorker(
            Fanex.Logging.ILogger logger,
            ILogger<SyncRedmineDataWorker> logfile,
            List<JobSetting> jobSettings,
            IBaseService baseService) : base(logger, logfile)
        {
            jobSetting = JobHelper.GetJobConfig(jobSettings, JobConstants.SyncRedmineData);
            this.baseService = baseService;
            this.logfile = logfile;
        }

        public override async Task ExecuteJobAsync()
        {
            await SyncUser();
            await SyncProject();
            await SyncTimeEntries();
            await SyncCustomValue();
        }

        private async Task SyncUser()
        {
            var criteria = new GetLastUpdatedUserCriteria();
            var lastUpdated = await baseService.GetDataEntityAsync<DateTime>(criteria);

            var criteria2 = new GetUserByLastUpdatedCriteria
            {
                LastUpdated = lastUpdated
            };
            var users = await baseService.GetDataAsync<User>(criteria2);

            var criteria3 = new SyncUserCriteria
            {
                Users = users.ToList()
            };
            await baseService.ExecuteNonQueryAsync(criteria3);
        }

        private async Task SyncProject()
        {
            var criteria = new GetLastUpdatedProjectCriteria();
            var lastUpdated = await baseService.GetDataEntityAsync<DateTime>(criteria);

            var criteria2 = new GetProjectByLastUpdatedCriteria
            {
                LastUpdated = lastUpdated
            };
            var projects = await baseService.GetDataAsync<Project>(criteria2);

            var criteria3 = new SyncProjectCriteria
            {
                Projects = projects.ToList()
            };
            await baseService.ExecuteNonQueryAsync(criteria3);
        }

        private async Task SyncTimeEntries()
        {
            var criteria = new GetLastUpdatedTimeEntriesCriteria();
            var lastUpdated = await baseService.GetDataEntityAsync<DateTime>(criteria);

            var criteria2 = new GetTimeEntriesByLastUpdatedCriteria
            {
                LastUpdated = lastUpdated
            };
            var timeEntries = await baseService.GetDataAsync<TimeEntries>(criteria2);

            var criteria3 = new SyncTimeEntriesCriteria
            {
                TimeEntries = timeEntries.ToList()
            };
            await baseService.ExecuteNonQueryAsync(criteria3);
        }

        private async Task SyncCustomValue()
        {
            var criteria = new GetMaxCustomValueCriteria();
            var maxCustomValueId = await baseService.GetDataEntityAsync<int>(criteria);

            var criteria2 = new GetCustomValueByMinIdCriteria
            {
                MinCustomValueId = maxCustomValueId
            };
            var customValues = await baseService.GetDataAsync<CustomValue>(criteria2);

            var criteria3 = new SyncCustomValueCriteria
            {
                CustomValues = customValues.ToList()
            };
            await baseService.ExecuteNonQueryAsync(criteria3);
        }
    }
}
