namespace NexGoal.AutoService.Services
{
    using System;
    using Fanex.Logging;

    public class AppService : IAppService
    {
        private readonly ILogger logger;
        private readonly IPeriodService periodService;
        private readonly IPAService paService;
        private readonly IPAFocusService focusService;
        private readonly INominationService nominationService;
        private readonly IProbationService probationService;

        public AppService(
            ILogger logger,
            IPeriodService periodService,
            IPAService paService,
            IPAFocusService focusService,
            INominationService nominationService,
            IProbationService probationService)
        {
            this.logger = logger;
            this.periodService = periodService;
            this.paService = paService;
            this.focusService = focusService;
            this.nominationService = nominationService;
            this.probationService = probationService;
        }

        public void Run()
        {
            RunProbation();
            RunPeriod();
            RunPA();
            RunPAFocus();
            RunNomination();
        }

        private void RunProbation()
        {
            try
            {
                probationService.SendMail();
                probationService.SendNotification();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
        }

        private void RunPeriod()
        {
            try
            {
                periodService.CreatePeriod();
                periodService.CreateEmployeeSettings();
                periodService.GeneratePA();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
        }

        private void RunPA()
        {
            try
            {
                paService.EditStatus();
                paService.EditGrade();
                paService.EditWhenEvaluationFinished();
                paService.SendMail();
                paService.SendNotification();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
        }

        private void RunPAFocus()
        {
            try
            {
                focusService.Generate();
                focusService.SendMail();
                focusService.SendNotification();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
        }

        private void RunNomination()
        {
            try
            {
                nominationService.SendMail();
                nominationService.SendNotification();
                nominationService.EditWhenFinished();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
        }
    }
}