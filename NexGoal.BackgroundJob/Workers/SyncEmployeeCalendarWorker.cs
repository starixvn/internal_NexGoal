namespace NexGoal.BackgroundJob.Workers
{
    using NexGoal.BackgroundJob.Criteria.Hrm;
    using NexGoal.BackgroundJob.Services;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;

    public class SyncEmployeeCalendarWorker : BaseWorker
    {
        private readonly IBaseService baseService;
        private readonly IEmployeeCalendarService employeeCalendarService;
        private readonly ILogger<SyncEmployeeCalendarWorker> logfile;

        public SyncEmployeeCalendarWorker(
            Fanex.Logging.ILogger logger,
            ILogger<SyncEmployeeCalendarWorker> logfile,
            List<JobSetting> jobSettings,
            IBaseService baseService,
            IEmployeeCalendarService employeeCalendarService) : base(logger, logfile)
        {
            jobSetting = JobHelper.GetJobConfig(jobSettings, JobConstants.SyncEmployeeCalendarJob);
            this.baseService = baseService;
            this.employeeCalendarService = employeeCalendarService;
            this.logfile = logfile;
        }

        public override async Task ExecuteJobAsync()
        {
            var lastModifiedDate = await employeeCalendarService.GetLastModifiedDateAsync();
            var calendars = await employeeCalendarService.GetCalendarsAsync(lastModifiedDate);
            var result = new CommandResult { Code = 0 };
            if (calendars.Any())
            {
                result = await employeeCalendarService.ImportCalendarsAsync(new SyncEmployeeCalendarCriteria { Calendars = calendars.ToList() });
            }
        }
    }
}
