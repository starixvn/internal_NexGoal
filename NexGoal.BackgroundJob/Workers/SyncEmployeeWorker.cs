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

        public SyncEmployeeWorker(
            Fanex.Logging.ILogger logger,
            List<JobSetting> jobSettings,
            IBaseService baseService,
            IEmployeeService employeeService) : base(logger)
        {
            jobSetting = JobHelper.GetJobConfig(jobSettings, JobConstants.SyncEmployeeJob);
            this.baseService = baseService;
            this.employeeService = employeeService;
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
