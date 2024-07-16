namespace NexGoal.BackgroundJob.Workers
{
    using Microsoft.Extensions.Options;
    using Services;

    public class SyncPeriodDataWorker : BaseWorker
    {
        private readonly IPeriodService periodService;

        public SyncPeriodDataWorker(Fanex.Logging.ILogger logger,
            IOptions<List<JobSetting>> jobSettings,
            IPeriodService periodService) : base(logger)
        {
            jobSetting = JobHelper.GetJobConfig(jobSettings.Value, JobConstants.SyncPeriodData);
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
