namespace NexGoal.BackgroundJob.Workers
{
    using NexGoal.BackgroundJob.Criteria.Hrm;
    using NexGoal.BackgroundJob.Services;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;

    public class SyncEmployeeAttendanceWorker : BaseWorker
    {
        private readonly IBaseService baseService;
        private readonly IEmployeeAttendanceService employeeAttendanceService;
        private readonly ILogger<SyncEmployeeAttendanceWorker> logfile;

        public SyncEmployeeAttendanceWorker(
            Fanex.Logging.ILogger logger,
            ILogger<SyncEmployeeAttendanceWorker> logfile,
            List<JobSetting> jobSettings,
            IBaseService baseService,
            IEmployeeAttendanceService employeeAttendanceService) : base(logger, logfile)
        {
            jobSetting = JobHelper.GetJobConfig(jobSettings, JobConstants.SyncEmployeeAttendanceJob);
            this.baseService = baseService;
            this.employeeAttendanceService = employeeAttendanceService;
            this.logfile = logfile;
        }

        public override async Task ExecuteJobAsync()
        {
            var lastModifiedDate = await employeeAttendanceService.GetLastModifiedDateAsync();
            var attendances = await employeeAttendanceService.GetAttendancesAsync(lastModifiedDate);
            var result = new CommandResult { Code = 0 };
            if (attendances.Any())
            {
                result = await employeeAttendanceService.ImportAttendancesAsync(new SyncEmployeeAttendanceCriteria { Attendances = attendances.ToList() });
            }
        }
    }
}
