namespace NexGoal.BackgroundJob.Workers
{
    using NexGoal.BackgroundJob.Services;

    public class NominationWorker : BaseWorker
    {
        private readonly INominationService nominationService;

        public NominationWorker(
            Fanex.Logging.ILogger logger,
            List<JobSetting> jobSettings,
            INominationService nominationService) : base(logger)
        {
            jobSetting = JobHelper.GetJobConfig(jobSettings, JobConstants.NominationJob);
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
