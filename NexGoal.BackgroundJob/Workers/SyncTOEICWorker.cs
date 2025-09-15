namespace NexGoal.BackgroundJob.Workers
{
    using NexGoal.BackgroundJob.Criteria.Hrm;
    using NexGoal.BackgroundJob.Services;

    public class SyncTOEICWorker : BaseWorker
    {
        private readonly ITOEICService toeicService;

        public SyncTOEICWorker(
            Fanex.Logging.ILogger logger,
            ILogger<SyncTOEICWorker> logfile,
            List<JobSetting> jobSettings,
            ITOEICService toeicService) : base(logger, logfile)
        {
            jobSetting = JobHelper.GetJobConfig(jobSettings, JobConstants.SyncTOEICJob);
            this.toeicService = toeicService;
        }

        public override async Task ExecuteJobAsync()
        {
            var lastModifiedDate = await toeicService.GetLastModifiedDateAsync();
            var toeics = await toeicService.GetTOEICsAsync(lastModifiedDate);
            if (toeics.Any())
            {
                await toeicService.ImportTOEICsAsync(new SyncTOEICCriteria { TOEICs = toeics.ToList() });
            }
        }
    }
}