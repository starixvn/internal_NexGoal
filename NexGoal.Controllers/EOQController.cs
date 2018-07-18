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

    public class EOQController : Controller
    {
        private readonly ISessionService sessionService;
        private readonly IEOQService eoqService;

        public EOQController(
            ISessionService sessionService,
            IEOQService eoqService)
        {
            this.sessionService = sessionService;
            this.eoqService = eoqService;
        }

        [NexGoalPermission(FunctionKey = FunctionKey.NominationManagement, Policy = Policy.Read)]
        public async Task<IActionResult> GetAll(int? year = null, int? statusId = null)
        {
            IEnumerable<EOQYModel> eoqs = await eoqService.GetAllEOQAsync(year, statusId).ConfigureAwait(false);
            IEnumerable<EOQYViewModel> viewModels = eoqs != null ? eoqs.Select(x => new EOQYViewModel
            {
                EOQYId = x.EOQYId,
                Year = x.Year,
                Quarter = x.Quarter,
                Description = x.Description,
                StartDateForNominator = x.StartDateForNominator,
                EndDateForNominator = x.EndDateForNominator,
                RemindDateForNominator = x.RemindDateForNominator,
                EndDateForCommittee = x.EndDateForCommittee
            }) : new List<EOQYViewModel>();

            return Json(viewModels);
        }

        [NexGoalPermission(FunctionKey = FunctionKey.NominationManagement, Policy = Policy.Read)]
        public async Task<IActionResult> GetYears()
        {
            IEnumerable<int> years = await eoqService.GetYearsAsync().ConfigureAwait(false);

            return Json(years);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.NominationManagement, Policy = Policy.Host)]
        public async Task<IActionResult> Create(CreateEOQModel eoq)
        {
            CommandResult commandResult = await eoqService.CreateEOQAsync(eoq).ConfigureAwait(false);

            return Json(commandResult);
        }

        [NexGoalPermission(FunctionKey = FunctionKey.NominationManagement, Policy = Policy.Host)]
        public async Task<IActionResult> Edit(int id)
        {
            EOQYModel eoq = await eoqService.GetEOQByIdAsync(id).ConfigureAwait(false);

            if (eoq == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            bool isHost = sessionService.HasPermissionOnFunction(FunctionKey.NominationManagement, Policy.Host);
            ViewData["IsHost"] = isHost && eoq.Year >= DateTime.Now.Year;

            return View(id);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.NominationManagement, Policy = Policy.Host)]
        public async Task<IActionResult> Edit(EOQYModel eoq)
        {
            CommandResult commandResult = await eoqService.EditEOQAsync(eoq).ConfigureAwait(false);

            return Json(commandResult);
        }

        [NexGoalPermission(FunctionKey = FunctionKey.NominationManagement, Policy = Policy.Read)]
        public async Task<IActionResult> Details(int id)
        {
            EOQYModel eoq = await eoqService.GetEOQByIdAsync(id).ConfigureAwait(false);

            if (eoq == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            return View(id);
        }

        [NexGoalPermission(FunctionKey = FunctionKey.NominationManagement, Policy = Policy.Read)]
        public async Task<IActionResult> GetById(int id)
        {
            EOQYModel eoq = await eoqService.GetEOQByIdAsync(id).ConfigureAwait(false);

            if (eoq == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            return Json(eoq);
        }

        [HttpDelete]
        [NexGoalPermission(FunctionKey = FunctionKey.NominationManagement, Policy = Policy.Host)]
        public async Task<IActionResult> Delete(int id)
        {
            CommandResult commandResult = await eoqService.DeleteEOQAsync(id).ConfigureAwait(false);

            return Json(commandResult);
        }
    }
}