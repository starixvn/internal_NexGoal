using Cronos;

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
                        var cronExpression = CronExpression.Parse(jobSetting.CronExpression);
                        var currentUtcTime = DateTimeOffset.UtcNow.UtcDateTime;
                        var occurenceTime = cronExpression.GetNextOccurrence(currentUtcTime);

                        var delay = occurenceTime.GetValueOrDefault() - currentUtcTime;
                        await Task.Delay(delay, stoppingToken);

                        await ExecuteJobAsync();
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());
                    }
                }
            }
        }

        public abstract Task ExecuteJobAsync();
    }
}
