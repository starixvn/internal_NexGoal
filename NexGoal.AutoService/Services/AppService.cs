namespace NexGoal.AutoService.Services
{
    using System;
    using Serilog;

    public class AppService : IAppService
    {
        private readonly IAvatarService avatarService;
        private readonly IPeriodService periodService;
        private readonly IPerformanceRecordService performanceRecordService;
        private readonly IPAService paService;
        private readonly INominationService nominationService;

        public AppService(
            IAvatarService avatarService,
            IPeriodService periodService,
            IPerformanceRecordService performanceRecordService,
            IPAService paService,
            INominationService nominationService)
        {
            this.avatarService = avatarService;
            this.periodService = periodService;
            this.performanceRecordService = performanceRecordService;
            this.paService = paService;
            this.nominationService = nominationService;
        }

        public void Run()
        {
            try
            {
                avatarService.Update();

                periodService.CreatePeriod();
                periodService.CreateEmployeeSettings();
                periodService.GeneratePA();

                paService.EditWhenEvaluationFinished();
                paService.EditStatus();

                nominationService.EditWhenFinished();

                performanceRecordService.EditStatus();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}