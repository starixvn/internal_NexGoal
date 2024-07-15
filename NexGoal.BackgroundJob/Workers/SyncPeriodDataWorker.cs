namespace NexGoal.BackgroundJob.Workers
{
    using AutoService.Services;
    using Microsoft.Extensions.Options;

    public class SyncPeriodDataWorker : BaseWorker
    {
        private readonly IPeriodService periodService;

        public SyncPeriodDataWorker(Fanex.Logging.ILogger logger,
            IOptions<List<JobSetting>> jobSettings,
            IPeriodService periodService) : base(logger)
        {
            jobSetting = JobHelper.GetJobConfig(jobSettings.Value, JobConstants.SyncPeriodDataWorker);
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
