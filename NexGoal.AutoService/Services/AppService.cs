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

        public AppService(
            IAvatarService avatarService,
            IPeriodService periodService,
            IPerformanceRecordService performanceRecordService,
            IPAService paService)
        {
            this.avatarService = avatarService;
            this.periodService = periodService;
            this.performanceRecordService = performanceRecordService;
            this.paService = paService;
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