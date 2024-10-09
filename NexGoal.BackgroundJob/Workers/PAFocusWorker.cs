namespace NexGoal.BackgroundJob.Workers
{
    using NexGoal.BackgroundJob.Services;

    public class PAFocusWorker : BaseWorker
    {
        private readonly IPAFocusService focusService;

        public PAFocusWorker(
            Fanex.Logging.ILogger logger,
            List<JobSetting> jobSettings,
            IPAFocusService focusService) : base(logger)
        {
            jobSetting = JobHelper.GetJobConfig(jobSettings, JobConstants.PAFocusJob);
            this.focusService = focusService;
        }

        public override async Task ExecuteJobAsync()
        {
            await focusService.GenerateAsync();
            await focusService.SendMailAsync();
            await focusService.SendNotificationAsync();

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(async () => await focusService.SendSkypeMessage());
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
    }
}
