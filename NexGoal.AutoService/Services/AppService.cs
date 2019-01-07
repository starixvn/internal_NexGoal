namespace NexGoal.AutoService.Services
{
    using System;
    using Fanex.Logging;

    public class AppService : IAppService
    {
        private readonly ILogger logger;
        private readonly IPeriodService periodService;
        private readonly IPerformanceRecordService performanceRecordService;
        private readonly IPAService paService;
        private readonly IPAFocusService focusService;
        private readonly INominationService nominationService;

        public AppService(
            ILogger logger,
            IPeriodService periodService,
            IPerformanceRecordService performanceRecordService,
            IPAService paService,
            IPAFocusService focusService,
            INominationService nominationService)
        {
            this.logger = logger;
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

                paService.SendMail();
                paService.EditGrade();
                paService.EditWhenEvaluationFinished();
                paService.EditStatus();

                focusService.Generate();
                focusService.SendMail();

                nominationService.SendMail();
                nominationService.EditWhenFinished();
            }
            catch (Exception ex)
            {
                logger.Error("Host terminated unexpectedly", ex);
            }
        }
    }
}