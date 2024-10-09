namespace NexGoal.BackgroundJob.Workers
{
    using Cronos;

    public abstract class BaseWorker(Fanex.Logging.ILogger logger) : BackgroundService
    {
        protected JobSetting jobSetting;

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
                        await logger.ErrorAsync(ex.ToString());
                    }
                }
            }
        }

        public abstract Task ExecuteJobAsync();
    }
}
