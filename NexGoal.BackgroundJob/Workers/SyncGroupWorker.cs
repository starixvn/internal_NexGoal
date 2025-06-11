namespace NexGoal.BackgroundJob.Workers
{
    using NexGoal.BackgroundJob.Criteria.Hrm;
    using NexGoal.BackgroundJob.Services;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;

    public class SyncGroupWorker : BaseWorker
    {
        private readonly IBaseService baseService;
        private readonly IGroupService groupService;
        private readonly ILogger<SyncGroupWorker> logfile;

        public SyncGroupWorker(
            Fanex.Logging.ILogger logger,
            ILogger<SyncGroupWorker> logfile,
            List<JobSetting> jobSettings,
            IBaseService baseService,
            IGroupService groupService) : base(logger, logfile)
        {
            jobSetting = JobHelper.GetJobConfig(jobSettings, JobConstants.SyncGroupJob);
            this.baseService = baseService;
            this.groupService = groupService;
            this.logfile = logfile;
        }

        public override async Task ExecuteJobAsync()
        {
            var lastModifiedDate = await groupService.GetLastModifiedDateAsync();
            var groups = await groupService.GetGroupsAsync(lastModifiedDate);
            var result = new CommandResult{ Code = 0 };
            if(groups.Any())
            {
                result = await groupService.ImportGroupsAsync(new SyncGroupCriteria { Groups = groups.ToList() });
            }
        }

    }
}
