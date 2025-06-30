namespace NexGoal.BackgroundJob.Workers;

using Criteria.Hrm;
using Services;

public class SyncGroupWorker : BaseWorker
{
    private readonly IGroupService groupService;

    public SyncGroupWorker(
        Fanex.Logging.ILogger logger,
        ILogger<SyncGroupWorker> logfile,
        List<JobSetting> jobSettings,
        IGroupService groupService) : base(logger, logfile)
    {
        jobSetting = JobHelper.GetJobConfig(jobSettings, JobConstants.SyncGroupJob);
        this.groupService = groupService;
    }

    public override async Task ExecuteJobAsync()
    {
        var lastModifiedDate = await groupService.GetLastModifiedDateAsync();
        var groups = await groupService.GetGroupsAsync(lastModifiedDate);
        if (groups.Any())
        {
            await groupService.ImportGroupsAsync(new SyncGroupCriteria { Groups = groups.ToList() });
        }
    }
}