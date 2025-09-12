namespace NexGoal.BackgroundJob.Workers
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using NexGoal.BackgroundJob.Criteria.Hrm;
    using NexGoal.BackgroundJob.Services.Interfaces;
    using NexGoal.Core.Models;

    public class JobTitleSyncWorker : BaseWorker
    {
        private readonly IJobTitleService jobTitleService;

        public JobTitleSyncWorker(
            Fanex.Logging.ILogger logger,
            ILogger<JobTitleSyncWorker> logfile,
            List<JobSetting> jobSettings,
            IJobTitleService jobTitleService) : base(logger, logfile)
        {
            jobSetting = JobHelper.GetJobConfig(jobSettings, JobConstants.SyncJobTitleJob);
            this.jobTitleService = jobTitleService;
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