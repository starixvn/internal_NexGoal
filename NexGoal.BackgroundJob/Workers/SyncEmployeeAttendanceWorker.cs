namespace NexGoal.BackgroundJob.Workers
{
    using NexGoal.BackgroundJob.Criteria.Hrm;
    using NexGoal.BackgroundJob.Services;

    public class SyncEmployeeAttendanceWorker : BaseWorker
    {
        private readonly IEmployeeAttendanceService employeeAttendanceService;

        public SyncEmployeeAttendanceWorker(
            Fanex.Logging.ILogger logger,
            ILogger<SyncEmployeeAttendanceWorker> logfile,
            List<JobSetting> jobSettings,
            IEmployeeAttendanceService employeeAttendanceService) : base(logger, logfile)
        {
            jobSetting = JobHelper.GetJobConfig(jobSettings, JobConstants.SyncEmployeeAttendanceJob);
            this.employeeAttendanceService = employeeAttendanceService;
        }

        public override async Task ExecuteJobAsync()
        {
            var lastModifiedDate = await employeeAttendanceService.GetLastModifiedDateAsync();
            var attendances = await employeeAttendanceService.GetAttendancesAsync(lastModifiedDate);
            if (attendances.Any())
            {
                await employeeAttendanceService.ImportAttendancesAsync(new SyncEmployeeAttendanceCriteria { Attendances = attendances.ToList() });
            }
        }
    }
}