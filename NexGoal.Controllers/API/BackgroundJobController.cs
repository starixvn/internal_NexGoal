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

        public async Task<ActionResult> Run()
        {
            await RunProbationAsync();
            await RunPeriodAsync();
            await RunPAAsync();
            await RunPAFocusAsync();
            await RunNominationAsync();

            return Ok("Successfully!");
        }

        private async Task RunProbationAsync()
        {
            try
            {
                await probationService.AddNewEmployeeToProbation();
                await probationService.SendSkypeMessage();
                await probationService.SendMailAsync();
                await probationService.SendNotificationAsync();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
        }

        private async Task RunPeriodAsync()
        {
            try
            {
                await periodService.CreatePeriodAsync();
                await periodService.CreateEmployeeSettingsAsync();
                await periodService.GeneratePAAsync();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
        }

        private async Task RunPAAsync()
        {
            try
            {
                await paService.EditStatusAsync();
                await paService.EditGradeAsync();
                await paService.EditWhenEvaluationFinishedAsync();
                await paService.SendMailAsync();
                await paService.SendNotificationAsync();
                await paService.SendSkypeMessage();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
        }

        private async Task RunPAFocusAsync()
        {
            try
            {
                await focusService.GenerateAsync();
                await focusService.SendMailAsync();
                await focusService.SendNotificationAsync();
                await focusService.SendSkypeMessage();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
        }

        private async Task RunNominationAsync()
        {
            try
            {
                await nominationService.SendMailAsync();
                await nominationService.SendNotificationAsync();
                await nominationService.EditWhenFinishedAsync();
                await nominationService.SendSkypeMessageAsync();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
        }
    }
}