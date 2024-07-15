namespace NexGoal.BackgroundJob.Workers
{
    using Microsoft.Extensions.Options;
    using NexGoal.AutoService.Services;

    public class SyncPADataWorker : BaseWorker
    {
        private readonly IPAService paService;

        public SyncPADataWorker(Fanex.Logging.ILogger logger,
            IOptions<List<JobSetting>> jobSettings,
            IPAService paService) : base(logger)
        {
            jobSetting = JobHelper.GetJobConfig(jobSettings.Value, JobConstants.SyncPADataWorker);
            this.paService = paService;
        }

        public override async Task ExecuteJobAsync()
        {
            await paService.EditStatusAsync();
            await paService.EditGradeAsync();
            await paService.EditWhenEvaluationFinishedAsync();
            await paService.SendMailAsync();
            await paService.SendNotificationAsync();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(async () => await paService.SendSkypeMessage());
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
    }
}
