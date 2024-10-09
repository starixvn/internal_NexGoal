namespace NexGoal.BackgroundJob.Workers
{
    using NexGoal.BackgroundJob.Services;

    public class PeriodWorker : BaseWorker
    {
        private readonly IPeriodService periodService;

        public PeriodWorker(Fanex.Logging.ILogger logger,
            List<JobSetting> jobSettings,
            IPeriodService periodService) : base(logger)
        {
            jobSetting = JobHelper.GetJobConfig(jobSettings, JobConstants.PeriodJob);
            this.periodService = periodService;
        }

        public override async Task ExecuteJobAsync()
        {
            await periodService.CreatePeriodAsync();
            await periodService.CreateEmployeeSettingsAsync();
            await periodService.GeneratePAAsync();
        }
    }
}
