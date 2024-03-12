namespace NexGoal.BackgroundJob.Workers
{
    public abstract class BaseWorker : BackgroundService
    {
        private readonly Fanex.Logging.ILogger logger;
        protected JobSetting? jobSetting;

        protected BaseWorker(Fanex.Logging.ILogger logger)
        {
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (jobSetting != null && jobSetting.IsEnabled)
                {
                    try
                    {
                        await ExecuteJobAsync();
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());
                    }

                    await Task.Delay(TimeSpan.FromSeconds(jobSetting.TimeInterval), stoppingToken);
                }
            }
        }

        public abstract Task ExecuteJobAsync();
    }
}
