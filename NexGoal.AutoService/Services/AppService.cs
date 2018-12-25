namespace NexGoal.AutoService.Services
{
    using System;
    using Serilog;

    public class AppService : IAppService
    {
        private readonly IPeriodService periodService;
        private readonly IPerformanceRecordService performanceRecordService;
        private readonly IPAService paService;
        private readonly IPAFocusService focusService;
        private readonly INominationService nominationService;

        public AppService(
            IPeriodService periodService,
            IPerformanceRecordService performanceRecordService,
            IPAService paService,
            IPAFocusService focusService,
            INominationService nominationService)
        {
            this.periodService = periodService;
            this.performanceRecordService = performanceRecordService;
            this.paService = paService;
            this.focusService = focusService;
            this.nominationService = nominationService;
        }

        public void Run()
        {
            try
            {
                performanceRecordService.EditStatus();

                periodService.CreatePeriod();
                periodService.CreateEmployeeSettings();
                periodService.GeneratePA();

                paService.EditGrade();
                paService.EditWhenEvaluationFinished();
                paService.EditStatus();
                paService.SendMail();

                focusService.Generate();
                focusService.SendMail();

                nominationService.SendMail();
                nominationService.EditWhenFinished();
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