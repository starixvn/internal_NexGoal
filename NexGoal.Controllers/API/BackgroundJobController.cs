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
                await probationService.SendMailAsync();
                await probationService.SendNotificationAsync();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                Task.Run(async () => await probationService.SendSkypeMessage());
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
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
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                Task.Run(async () => await paService.SendSkypeMessage());
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
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
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                Task.Run(async () => await focusService.SendSkypeMessage());
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
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
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                Task.Run(async () => await nominationService.SendSkypeMessageAsync());
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
        }
    }
}