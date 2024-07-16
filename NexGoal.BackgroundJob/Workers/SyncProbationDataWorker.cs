namespace NexGoal.BackgroundJob.Workers
{
    using Microsoft.Extensions.Options;
    using Services;

    public class SyncProbationDataWorker : BaseWorker
    {
        private readonly IProbationService probationService;

        public SyncProbationDataWorker(Fanex.Logging.ILogger logger,
            IOptions<List<JobSetting>> jobSettings,
            IProbationService probationService) : base(logger)
        {
            jobSetting = JobHelper.GetJobConfig(jobSettings.Value, JobConstants.SyncProbationData);
            this.probationService = probationService;
        }

        public override async Task ExecuteJobAsync()
        {
            await probationService.AddNewEmployeeToProbation();
            await probationService.SendMailAsync();
            await probationService.SendNotificationAsync();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(async () => await probationService.SendSkypeMessage());
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
    }
}
