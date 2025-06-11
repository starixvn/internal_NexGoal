namespace NexGoal.BackgroundJob.Workers
{
    using NexGoal.BackgroundJob.Criteria.Hrm;
    using NexGoal.BackgroundJob.Services;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;

    public class SyncEmployeeWorker : BaseWorker
    {
        private readonly IBaseService baseService;
        private readonly IEmployeeService employeeService;
        private readonly ILogger<SyncEmployeeWorker> logfile;

        public SyncEmployeeWorker(
            Fanex.Logging.ILogger logger,
            ILogger<SyncEmployeeWorker> logfile,
            List<JobSetting> jobSettings,
            IBaseService baseService,
            IEmployeeService employeeService) : base(logger, logfile)
        {
            jobSetting = JobHelper.GetJobConfig(jobSettings, JobConstants.SyncEmployeeJob);
            this.baseService = baseService;
            this.employeeService = employeeService;
            this.logfile = logfile;
        }

        public override async Task ExecuteJobAsync()
        {
            var lastModifiedDate = await employeeService.GetLastModifiedDateAsync();
            var employees = await employeeService.GetEmployeesAsync(lastModifiedDate);
            var result = new CommandResult{ Code = 0 };
            if(employees.Any())
            {
                result = await employeeService.ImportEmployeesAsync(new SyncEmployeeCriteria { Employees = employees.ToList() });
            }
        }

    }
}
