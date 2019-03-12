namespace NexGoal.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Models.EOQY;
    using NexGoal.Services;
    using NexGoal.Web.Core.Filters;
    using NexGoal.Web.Core.Services;

    [NexGoalPermission(FunctionKey = FunctionKey.NominationPeriod, Policy = Policy.Read)]
    public class EOYController : BaseController
    {
        private readonly IEOYService eoyService;

        public EOYController(
            ISessionService sessionService,
            IEOYService eoyService) : base(sessionService)
        {
            this.eoyService = eoyService;
        }

        public async Task<JsonResult> GetAll(int? statusId = null)
        {
            IEnumerable<EOQYModel> eoys = await eoyService.GetAllAsync(statusId).ConfigureAwait(false);

            return Json(eoys);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.NominationPeriod, Policy = Policy.Host)]
        public async Task<JsonResult> Create(CreateEOYModel eoy)
        {
            if (ModelState.IsValid)
            {
                CommandResult commandResult = await eoyService.CreateAsync(sessionService.EmployeeId, eoy).ConfigureAwait(false);

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });
            return Json(new { isSuccessful = false, errors });
        }

        [NexGoalPermission(FunctionKey = FunctionKey.NominationPeriod, Policy = Policy.Host)]
        public async Task<IActionResult> Edit(int id)
        {
            EOQYModel eoy = await eoyService.GetByIdAsync(id).ConfigureAwait(false);

            if (eoy == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            bool isHost = sessionService.HasPermissionOnFunction(FunctionKey.NominationPeriod, Policy.Host);
            ViewData["IsHost"] = isHost && eoy.Year >= DateTime.Now.Year;

            return View(id);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.NominationPeriod, Policy = Policy.Host)]
        public async Task<JsonResult> Edit(EditEOYModel eoy)
        {
            if (ModelState.IsValid)
            {
                CommandResult commandResult = await eoyService.EditAsync(sessionService.EmployeeId, eoy).ConfigureAwait(false);

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });
            return Json(new { isSuccessful = false, errors });
        }

        public async Task<IActionResult> Details(int id)
        {
            EOQYModel eoy = await eoyService.GetByIdAsync(id).ConfigureAwait(false);

            if (eoy == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            return View(id);
        }

        public async Task<JsonResult> GetById(int id)
        {
            EOQYModel eoy = await eoyService.GetByIdAsync(id).ConfigureAwait(false);

            return Json(eoy);
        }
    }
}