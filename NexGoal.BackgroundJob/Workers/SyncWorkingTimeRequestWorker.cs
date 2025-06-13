namespace NexGoal.BackgroundJob.Workers
{
    using NexGoal.BackgroundJob.Criteria.Hrm;
    using NexGoal.BackgroundJob.Services;
    using NexGoal.BackgroundJob.Services.Interface;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;

    public class SyncWorkingTimeRequestWorker : BaseWorker
    {
        private readonly IBaseService baseService;
        private readonly IWorkingTimeRequestService workingTimeRequestService;
        private readonly ILogger<SyncWorkingTimeRequestWorker> logfile;

        public SyncWorkingTimeRequestWorker(
            Fanex.Logging.ILogger logger,
            ILogger<SyncWorkingTimeRequestWorker> logfile,
            List<JobSetting> jobSettings,
            IBaseService baseService,
            IWorkingTimeRequestService workingTimeRequestService) : base(logger, logfile)
        {
            jobSetting = JobHelper.GetJobConfig(jobSettings, JobConstants.SyncWorkingTimeRequestJob);
            this.baseService = baseService;
            this.workingTimeRequestService = workingTimeRequestService;
            this.logfile = logfile;
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