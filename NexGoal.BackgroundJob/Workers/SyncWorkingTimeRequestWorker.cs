namespace NexGoal.BackgroundJob.Workers
{
    using NexGoal.BackgroundJob.Criteria.Hrm;
    using NexGoal.BackgroundJob.Services.Interfaces;

    public class SyncWorkingTimeRequestWorker : BaseWorker
    {
        private readonly IWorkingTimeRequestService workingTimeRequestService;

        public SyncWorkingTimeRequestWorker(
            Fanex.Logging.ILogger logger,
            ILogger<SyncWorkingTimeRequestWorker> logfile,
            List<JobSetting> jobSettings,
            IWorkingTimeRequestService workingTimeRequestService) : base(logger, logfile)
        {
            jobSetting = JobHelper.GetJobConfig(jobSettings, JobConstants.SyncWorkingTimeRequestJob);
            this.workingTimeRequestService = workingTimeRequestService;
        }

        public override async Task ExecuteJobAsync()
        {
            var lastModifiedDate = await workingTimeRequestService.GetLastModifiedDateAsync();
            var requests = await workingTimeRequestService.GetWorkingTimeRequestsAsync(lastModifiedDate);
            if (requests.Any())
            {
                await workingTimeRequestService.ImportWorkingTimeRequestsAsync(new SyncWorkingTimeRequestCriteria { WorkingTimeRequests = requests.ToList() });
            }
        }
    }
}