namespace NexGoal.BackgroundJob.Workers
{
    using Cronos;

    public abstract class BaseWorker(
        Fanex.Logging.ILogger logger,
        ILogger<BaseWorker> logfile) : BackgroundService
    {
        private readonly ILogger logfile = logfile;

        protected JobSetting jobSetting;

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            logfile.LogInformation($"{jobSetting.JobName}: Start");
            return base.StartAsync(cancellationToken);
        }

        private async Task RunJobAsync()
        {
            try
            {
                if (jobSetting.IsEnabled)
                {
                    logfile.LogInformation($"{jobSetting.JobName}: Doing...");
                    await ExecuteJobAsync();
                    logfile.LogInformation($"{jobSetting.JobName}: Done.");
                }
            }
            catch (Exception ex)
            {
                await logger.ErrorAsync($"{jobSetting.JobName}: Error - {ex}");
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                
                try
                {
                    var cronExpression = CronExpression.Parse(jobSetting.CronExpression);
                    var currentUtcTime = DateTimeOffset.UtcNow.UtcDateTime;
                    var occurenceTime = cronExpression.GetNextOccurrence(currentUtcTime);

                    var delay = occurenceTime.GetValueOrDefault() - currentUtcTime;
                    await Task.Delay(delay, stoppingToken);

                    await RunJobAsync();
                }
                catch (Exception ex)
                {
                    await logger.ErrorAsync(ex.ToString());
                }
            }
        }

        public virtual async Task ExecuteJobAsync()
        {
            await Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            logfile.LogInformation($"{jobSetting.JobName}: Stop");

            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            logfile.LogInformation($"{jobSetting.JobName}: Disposed");
            GC.SuppressFinalize(this);
            base.Dispose();
        }
    }
}
