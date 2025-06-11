namespace NexGoal.BackgroundJob.Workers
{
    using NexGoal.BackgroundJob.Services;

    public class PeriodWorker : BaseWorker
    {
        private readonly IPeriodService periodService;
        private readonly ILogger<PeriodWorker> logfile;

        public PeriodWorker(Fanex.Logging.ILogger logger,
            ILogger<PeriodWorker> logfile,
            List<JobSetting> jobSettings,
            IPeriodService periodService) : base(logger, logfile)
        {
            jobSetting = JobHelper.GetJobConfig(jobSettings, JobConstants.PeriodJob);
            this.periodService = periodService;
            this.logfile = logfile;
        }

        public override async Task ExecuteJobAsync()
        {
            await periodService.CreatePeriodAsync();
            await periodService.CreateEmployeeSettingsAsync();
            await periodService.GeneratePAAsync();
        }
    }
}
