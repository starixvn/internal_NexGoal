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
    public class EOQController : BaseController
    {
        private readonly IEOQService eoqService;

        public EOQController(
            ISessionService sessionService,
            IEOQService eoqService) : base(sessionService)
        {
            this.eoqService = eoqService;
        }

        public async Task<JsonResult> GetAll(int? year = null, int? statusId = null)
        {
            IEnumerable<EOQYModel> eoqs = await eoqService.GetAllAsync(year, statusId);

            return Json(eoqs);
        }

        public async Task<JsonResult> GetYears()
        {
            IEnumerable<int> years = await eoqService.GetYearsAsync();

            return Json(years);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.NominationPeriod, Policy = Policy.Host)]
        public async Task<JsonResult> Create(CreateEOQModel eoq)
        {
            if (ModelState.IsValid)
            {
                CommandResult commandResult = await eoqService.CreateAsync(sessionService.EmployeeId, eoq);

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors[0].ErrorMessage });
            return Json(new { isSuccessful = false, errors });
        }

        [NexGoalPermission(FunctionKey = FunctionKey.NominationPeriod, Policy = Policy.Host)]
        public async Task<IActionResult> Edit(int id)
        {
            EOQYModel eoq = await eoqService.GetByIdAsync(id);

            if (eoq == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            bool isHost = sessionService.HasPermissionOnFunction(FunctionKey.NominationPeriod, Policy.Host);
            ViewData["IsHost"] = isHost && (eoq.Year + 1) >= DateTimeOffset.Now.Year;

            return View(id);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.NominationPeriod, Policy = Policy.Host)]
        public async Task<JsonResult> Edit(EditEOQModel eoq)
        {
            if (ModelState.IsValid)
            {
                CommandResult commandResult = await eoqService.EditAsync(sessionService.EmployeeId, eoq);

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors[0].ErrorMessage });
            return Json(new { isSuccessful = false, errors });
        }

        public async Task<IActionResult> Details(int id)
        {
            EOQYModel eoq = await eoqService.GetByIdAsync(id);

            if (eoq == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            return View(id);
        }

        public async Task<JsonResult> GetById(int id)
        {
            EOQYModel eoq = await eoqService.GetByIdAsync(id);

            return Json(eoq);
        }
    }
}