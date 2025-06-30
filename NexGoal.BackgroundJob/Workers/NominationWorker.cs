namespace NexGoal.BackgroundJob.Workers
{
    using NexGoal.BackgroundJob.Services;

    public class NominationWorker : BaseWorker
    {
        private readonly INominationService nominationService;
        private readonly ILogger<NominationWorker> logfile;

        public NominationWorker(
            Fanex.Logging.ILogger logger,
            ILogger<NominationWorker> logfile,
            List<JobSetting> jobSettings,
            INominationService nominationService) : base(logger, logfile)
        {
            jobSetting = JobHelper.GetJobConfig(jobSettings, JobConstants.NominationJob);
            this.nominationService = nominationService;
            this.logfile = logfile;
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
