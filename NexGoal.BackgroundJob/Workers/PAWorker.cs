namespace NexGoal.BackgroundJob.Workers
{
    using NexGoal.BackgroundJob.Services;

    public class PAWorker : BaseWorker
    {
        private readonly IPAService paService;

        public PAWorker(Fanex.Logging.ILogger logger,
            List<JobSetting> jobSettings,
            IPAService paService) : base(logger)
        {
            jobSetting = JobHelper.GetJobConfig(jobSettings, JobConstants.PAJob);
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
            Task.Run(async () => await paService.SendSkypeMessageAsync());
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
    }
}
