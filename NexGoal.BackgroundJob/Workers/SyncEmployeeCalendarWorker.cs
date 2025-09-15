namespace NexGoal.BackgroundJob.Workers
{
    using NexGoal.BackgroundJob.Criteria.Hrm;
    using NexGoal.BackgroundJob.Services;

    public class SyncEmployeeCalendarWorker : BaseWorker
    {
        private readonly IEmployeeCalendarService employeeCalendarService;

        public SyncEmployeeCalendarWorker(
            Fanex.Logging.ILogger logger,
            ILogger<SyncEmployeeCalendarWorker> logfile,
            List<JobSetting> jobSettings,
            IEmployeeCalendarService employeeCalendarService) : base(logger, logfile)
        {
            jobSetting = JobHelper.GetJobConfig(jobSettings, JobConstants.SyncEmployeeCalendarJob);
            this.employeeCalendarService = employeeCalendarService;
        }

        public override async Task ExecuteJobAsync()
        {
            var lastModifiedDate = await employeeCalendarService.GetLastModifiedDateAsync();
            var calendars = await employeeCalendarService.GetCalendarsAsync(lastModifiedDate);
            if (calendars.Any())
            {
                await employeeCalendarService.ImportCalendarsAsync(new SyncEmployeeCalendarCriteria { Calendars = calendars.ToList() });
            }
        }
    }
}