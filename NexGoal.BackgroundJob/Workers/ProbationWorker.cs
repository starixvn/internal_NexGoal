namespace NexGoal.BackgroundJob.Workers
{
    using NexGoal.BackgroundJob.Services;

    public class ProbationWorker : BaseWorker
    {
        private readonly IProbationService probationService;

        public ProbationWorker(
            Fanex.Logging.ILogger logger,
            ILogger<ProbationWorker> logfile,
            List<JobSetting> jobSettings,
            IProbationService probationService) : base(logger, logfile)
        {
            jobSetting = JobHelper.GetJobConfig(jobSettings, JobConstants.ProbationJob);
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