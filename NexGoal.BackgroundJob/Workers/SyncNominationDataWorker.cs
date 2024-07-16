namespace NexGoal.BackgroundJob.Workers
{
    using Microsoft.Extensions.Options;
    using Services;

    public class SyncNominationDataWorker : BaseWorker
    {
        private readonly INominationService nominationService;

        public SyncNominationDataWorker(Fanex.Logging.ILogger logger,
            IOptions<List<JobSetting>> jobSettings,
            INominationService nominationService) : base(logger)
        {
            jobSetting = JobHelper.GetJobConfig(jobSettings.Value, JobConstants.SyncNominationData);
            this.nominationService = nominationService;
        }

        public override async Task ExecuteJobAsync()
        {
            await nominationService.SendMailAsync();
            await nominationService.SendNotificationAsync();
            await nominationService.EditWhenFinishedAsync();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(async () => await nominationService.SendSkypeMessageAsync());
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
    }
}
