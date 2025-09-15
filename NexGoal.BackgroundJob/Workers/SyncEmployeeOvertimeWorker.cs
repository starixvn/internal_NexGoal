namespace NexGoal.BackgroundJob.Workers
{
    using NexGoal.BackgroundJob.Criteria.Hrm;
    using NexGoal.BackgroundJob.Services;

    public class SyncEmployeeOvertimeWorker : BaseWorker
    {
        private readonly IEmployeeOvertimeService employeeOvertimeService;

        public SyncEmployeeOvertimeWorker(
            Fanex.Logging.ILogger logger,
            ILogger<SyncEmployeeOvertimeWorker> logfile,
            List<JobSetting> jobSettings,
            IEmployeeOvertimeService employeeOvertimeService) : base(logger, logfile)
        {
            jobSetting = JobHelper.GetJobConfig(jobSettings, JobConstants.SyncEmployeeOvertimeJob);
            this.employeeOvertimeService = employeeOvertimeService;
        }

        public override async Task ExecuteJobAsync()
        {
            var lastModifiedDate = await employeeOvertimeService.GetLastModifiedDateAsync();
            var overtimes = await employeeOvertimeService.GetOvertimesAsync(lastModifiedDate);
            if (overtimes.Any())
            {
                await employeeOvertimeService.ImportOvertimesAsync(new SyncEmployeeOvertimeCriteria { Overtimes = overtimes.ToList() });
            }
        }
    }
}