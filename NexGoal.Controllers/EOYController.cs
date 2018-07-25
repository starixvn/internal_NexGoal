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
            IEnumerable<EOQYModel> eoys = await eoyService.GetAllAsync(statusId).ConfigureAwait(false);
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
            if (ModelState.IsValid)
            {
                CommandResult commandResult = await eoyService.CreateAsync(eoy).ConfigureAwait(false);

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });
            return Json(new { isSuccessful = false, errors });
        }

        [NexGoalPermission(FunctionKey = FunctionKey.NominationManagement, Policy = Policy.Host)]
        public async Task<IActionResult> Edit(int id)
        {
            EOQYModel eoy = await eoyService.GetByIdAsync(id).ConfigureAwait(false);

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
        public async Task<IActionResult> Edit(EditEOYModel eoy)
        {
            if (ModelState.IsValid)
            {
                CommandResult commandResult = await eoyService.EditAsync(eoy).ConfigureAwait(false);

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });
            return Json(new { isSuccessful = false, errors });
        }

        [NexGoalPermission(FunctionKey = FunctionKey.NominationManagement, Policy = Policy.Read)]
        public async Task<IActionResult> Details(int id)
        {
            EOQYModel eoy = await eoyService.GetByIdAsync(id).ConfigureAwait(false);

            if (eoy == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            return View(id);
        }

        [NexGoalPermission(FunctionKey = FunctionKey.NominationManagement, Policy = Policy.Read)]
        public async Task<IActionResult> GetById(int id)
        {
            EOQYModel eoy = await eoyService.GetByIdAsync(id).ConfigureAwait(false);

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
            CommandResult commandResult = await eoyService.DeleteAsync(id).ConfigureAwait(false);

            return Json(commandResult);
        }

        [NexGoalPermission(FunctionKey = FunctionKey.NominationManagement, Policy = Policy.Read)]
        public async Task<IActionResult> GetNominatorSetting(
            int eoqId,
            string keyword = null,
            string groupIds = null,
            int? roleId = null,
            bool isShowAll = false)
        {
            IEnumerable<NominatorSettingModel> nominators = await eoyService.GetNominatorSettingAsync(eoqId, keyword, groupIds, roleId, isShowAll).ConfigureAwait(false);
            IEnumerable<NominatorSettingViewModel> viewModels = nominators.Select(x => new NominatorSettingViewModel
            {
                EmployeeId = x.EmployeeId,
                NickName = x.NickName,
                FullName = x.FullName,
                GroupName = x.GroupName,
                JobTitle = x.JobTitle,
                RoleIds = x.RoleIds
            });

            return Json(viewModels);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.NominationManagement, Policy = Policy.Host)]
        public async Task<IActionResult> EditNominatorSetting(EditNominatorSettingModel setting)
        {
            CommandResult commandResult = await eoyService.EditNominatorSettingAsync(setting).ConfigureAwait(false);

            return Json(commandResult);
        }
    }
}