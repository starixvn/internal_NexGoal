namespace NexGoal.AutoService.Services
{
    using System;
    using Serilog;

    public class AppService : IAppService
    {
        private readonly IPeriodService periodService;
        private readonly IPerformanceRecordService performanceRecordService;

        public AppService(
            IPeriodService periodService,
            IPerformanceRecordService performanceRecordService)
        {
            this.periodService = periodService;
            this.performanceRecordService = performanceRecordService;
        }

        public void Run()
        {
            try
            {
                periodService.InsertEmployeeSettings();
                periodService.GeneratePA();
                periodService.EditStatus();

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