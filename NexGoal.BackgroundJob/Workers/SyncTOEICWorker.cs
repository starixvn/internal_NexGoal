namespace NexGoal.BackgroundJob.Workers
{
    using NexGoal.BackgroundJob.Criteria.Hrm;
    using NexGoal.BackgroundJob.Services;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;

    public class SyncTOEICWorker : BaseWorker
    {
        private readonly IBaseService baseService;
        private readonly ITOEICService toeicService;
        private readonly ILogger<SyncTOEICWorker> logfile;

        public SyncTOEICWorker(
            Fanex.Logging.ILogger logger,
            ILogger<SyncTOEICWorker> logfile,
            List<JobSetting> jobSettings,
            IBaseService baseService,
            ITOEICService toeicService) : base(logger, logfile)
        {
            jobSetting = JobHelper.GetJobConfig(jobSettings, JobConstants.SyncTOEICJob);
            this.baseService = baseService;
            this.toeicService = toeicService;
            this.logfile = logfile;
        }

        public override async Task ExecuteJobAsync()
        {
            var lastModifiedDate = await toeicService.GetLastModifiedDateAsync();
            var toeics = await toeicService.GetTOEICsAsync(lastModifiedDate);
            var result = new CommandResult { Code = 0 };
            if (toeics.Any())
            {
                result = await toeicService.ImportTOEICsAsync(new SyncTOEICCriteria { TOEICs = toeics.ToList() });
            }
        }
    }
} 