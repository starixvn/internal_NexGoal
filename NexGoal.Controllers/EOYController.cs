namespace NexGoal.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Filters;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Models.EOQY;
    using NexGoal.Services;
    using NexGoal.ViewModels.EOQY;

    public class EOYController : Controller
    {
        private readonly ISessionService sessionService;
        private readonly IEOYService eoyService;

        public EOYController(
            ISessionService sessionService,
            IEOYService eoyService)
        {
            this.sessionService = sessionService;
            this.eoyService = eoyService;
        }

        [NexGoalPermission(FunctionKey = FunctionKey.NominationManagement, Policy = Policy.Read)]
        public async Task<IActionResult> GetAll(int? statusId = null)
        {
            IEnumerable<EOQYModel> eoys = await eoyService.GetAllEOYAsync(statusId).ConfigureAwait(false);
            IEnumerable<EOQYViewModel> viewModels = eoys != null ? eoys.Select(x => new EOQYViewModel
            {
                EOQYId = x.EOQYId,
                Year = x.Year,
                Description = x.Description,
                StartDateForNominator = x.StartDateForNominator,
                EndDateForNominator = x.EndDateForNominator,
                RemindDateForNominator = x.RemindDateForNominator,
                EndDateForCommittee = x.EndDateForCommittee
            }) : new List<EOQYViewModel>();

            return Json(viewModels);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.NominationManagement, Policy = Policy.Host)]
        public async Task<IActionResult> Create(CreateEOYModel eoy)
        {
            CommandResult commandResult = await eoyService.CreateEOYAsync(eoy).ConfigureAwait(false);

            return Json(commandResult);
        }

        [NexGoalPermission(FunctionKey = FunctionKey.NominationManagement, Policy = Policy.Host)]
        public async Task<IActionResult> Edit(int id)
        {
            EOQYModel eoy = await eoyService.GetEOYByIdAsync(id).ConfigureAwait(false);

            if (eoy == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            bool isHost = sessionService.HasPermissionOnFunction(FunctionKey.NominationManagement, Policy.Host);
            ViewData["IsHost"] = isHost && eoy.Year >= DateTime.Now.Year;

            return View(id);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.NominationManagement, Policy = Policy.Host)]
        public async Task<IActionResult> Edit(EOQYModel eoy)
        {
            CommandResult commandResult = await eoyService.EditEOYAsync(eoy).ConfigureAwait(false);

            return Json(commandResult);
        }

        [NexGoalPermission(FunctionKey = FunctionKey.NominationManagement, Policy = Policy.Read)]
        public async Task<IActionResult> Details(int id)
        {
            EOQYModel eoy = await eoyService.GetEOYByIdAsync(id).ConfigureAwait(false);

            if (eoy == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            return View(id);
        }

        [NexGoalPermission(FunctionKey = FunctionKey.NominationManagement, Policy = Policy.Read)]
        public async Task<IActionResult> GetById(int id)
        {
            EOQYModel eoy = await eoyService.GetEOYByIdAsync(id).ConfigureAwait(false);

            if (eoy == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            return Json(eoy);
        }

        [HttpDelete]
        [NexGoalPermission(FunctionKey = FunctionKey.NominationManagement, Policy = Policy.Host)]
        public async Task<IActionResult> Delete(int id)
        {
            CommandResult commandResult = await eoyService.DeleteEOYAsync(id).ConfigureAwait(false);

            return Json(commandResult);
        }
    }
}