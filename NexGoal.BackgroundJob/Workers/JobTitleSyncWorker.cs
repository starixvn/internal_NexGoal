namespace NexGoal.BackgroundJob.Workers
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using NexGoal.BackgroundJob.Criteria.Hrm;
    using NexGoal.BackgroundJob.Services.Interface;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;

    public class JobTitleSyncWorker : BaseWorker
    {
        private readonly IBaseService baseService;
        private readonly IJobTitleService jobTitleService;
        private readonly ILogger<JobTitleSyncWorker> logfile;

        public JobTitleSyncWorker(
            Fanex.Logging.ILogger logger,
            ILogger<JobTitleSyncWorker> logfile,
            List<JobSetting> jobSettings,
            IBaseService baseService,
            IJobTitleService jobTitleService) : base(logger, logfile)
        {
            jobSetting = JobHelper.GetJobConfig(jobSettings, JobConstants.SyncJobTitleJob);
            this.baseService = baseService;
            this.jobTitleService = jobTitleService;
            this.logfile = logfile;
        }

        public override async Task ExecuteJobAsync()
        {
            var jobTitles = await jobTitleService.GetJobTitlesAsync();
            var result = new CommandResult { Code = 0 };
            if (jobTitles.Any())
            {
                result = await jobTitleService.ImportJobTitlesAsync(new SyncJobTitleCriteria { JobTitles = jobTitles.ToList() });
            }
        }
    }
} 