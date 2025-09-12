namespace NexGoal.BackgroundJob.Workers
{
    using NexGoal.BackgroundJob.Criteria.Hrm;
    using NexGoal.BackgroundJob.Services;
    using NexGoal.Core.Models;

    public class SyncEmployeeWorker : BaseWorker
    {
        private readonly IEmployeeService employeeService;

        public SyncEmployeeWorker(
            Fanex.Logging.ILogger logger,
            ILogger<SyncEmployeeWorker> logfile,
            List<JobSetting> jobSettings,
            IEmployeeService employeeService) : base(logger, logfile)
        {
            jobSetting = JobHelper.GetJobConfig(jobSettings, JobConstants.SyncEmployeeJob);
            this.employeeService = employeeService;
        }

        public override async Task ExecuteJobAsync()
        {
            var lastModifiedDate = await employeeService.GetLastModifiedDateAsync();
            var employees = await employeeService.GetEmployeesAsync(lastModifiedDate);
            var result = new CommandResult { Code = 0 };
            if (employees.Any())
            {
                result = await employeeService.ImportEmployeesAsync(new SyncEmployeeCriteria { Employees = employees.ToList() });
            }
        }
    }
}