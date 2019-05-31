namespace NexGoal.Controllers.API
{
    using System;
    using System.Threading.Tasks;
    using Fanex.Logging;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.AutoService.Services;

    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class BackgroundJobController : ControllerBase
    {
        private readonly ILogger logger;
        private readonly IPeriodService periodService;
        private readonly IPAService paService;
        private readonly IPAFocusService focusService;
        private readonly INominationService nominationService;
        private readonly IProbationService probationService;

        public BackgroundJobController(
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

        public ActionResult Run()
        {
            RunProbation();
            RunPeriod();
            RunPA();
            RunPAFocus();
            RunNomination();

            return Ok("Successfully!");
        }

        private void RunProbation()
        {
            try
            {
                probationService.SendMail();
                probationService.SendNotification();
                Task.Run(() =>
                {
                    probationService.SendSkypeMessage();
                    probationService.AddNewEmployeeToProbation();
                });
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
                Task.Run(() => paService.SendSkypeMessage());
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
                Task.Run(() => focusService.SendSkypeMessage());
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
                Task.Run(() => nominationService.SendSkypeMessage());
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
        }
    }
}