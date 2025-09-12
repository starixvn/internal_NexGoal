namespace NexGoal.BackgroundJob.Workers
{
    using NexGoal.BackgroundJob.Criteria.Hrm;
    using NexGoal.BackgroundJob.Services.Interfaces;
    using NexGoal.Core.Models;

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
            var result = new CommandResult { Code = 0 };
            if (requests.Any())
            {
                result = await workingTimeRequestService.ImportWorkingTimeRequestsAsync(new SyncWorkingTimeRequestCriteria { WorkingTimeRequests = requests.ToList() });
            }
        }
    }
}