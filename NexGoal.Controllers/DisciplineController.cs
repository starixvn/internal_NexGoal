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
    using NexGoal.Models.Discipline;
    using NexGoal.Services;
    using NexGoal.ViewModels.Discipline;

    [NexGoalPermission(FunctionKey = FunctionKey.DisciplineManagement, Policy = Policy.Read)]
    public class DisciplineController : Controller
    {
        private readonly IDisciplineService disciplineService;
        private readonly ISessionService sessionService;

        public DisciplineController(IDisciplineService disciplineService, ISessionService sessionService)
        {
            this.disciplineService = disciplineService;
            this.sessionService = sessionService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(sessionService.HasPermissionOnFunction(FunctionKey.DisciplineManagement, Policy.Host));
        }

        [HttpGet]
        public async Task<JsonResult> GetDisciplines(DateTime from, DateTime to, bool? isActive, string groupIds, int? employeeId)
        {
            IEnumerable<DisciplineModel> disciplines = await disciplineService.GetDisciplines(from, to, isActive, groupIds, employeeId).ConfigureAwait(false);

            return Json(disciplines);
        }

        [HttpGet]
        public async Task<JsonResult> GetHRMSummary()
        {
            var from = new DateTime(DateTime.Now.Year, 1, 1);
            var to = DateTime.Now;
            var HRMSummay = await disciplineService.GetHRMSummary(from, to, sessionService.EmployeeId).ConfigureAwait(false);
            return Json(HRMSummay);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.DisciplineManagement, Policy = Policy.Host)]
        public async Task<JsonResult> EditStatus(int disciplineId, bool isActive)
        {
            CommandResult commandResult = await disciplineService.EditStatus(disciplineId, isActive, sessionService.EmployeeId).ConfigureAwait(false);

            return Json(commandResult);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.DisciplineManagement, Policy = Policy.Host)]
        public async Task<JsonResult> Create(AddDisciplineViewModel model)
        {
            if (ModelState.IsValid)
            {
                CommandResult commandResult = await disciplineService.Create(new AddDisciplineModel
                {
                    EmployeeId = model.EmployeeId,
                    Content = model.Content,
                    EffectiveDate = model.EffectiveDate,
                    CreatedBy = sessionService.EmployeeId
                }).ConfigureAwait(false);

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });
            return Json(new { isSuccessful = false, errors });
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.DisciplineManagement, Policy = Policy.Host)]
        public async Task<JsonResult> Edit(EditDisciplineViewModel model)
        {
            if (ModelState.IsValid)
            {
                CommandResult commandResult = await disciplineService.Edit(new EditDisciplineModel
                {
                    DisciplineId = model.DisciplineId,
                    EffectiveDate = model.EffectiveDate,
                    Content = model.Content,
                    ModifiedBy = sessionService.EmployeeId
                }).ConfigureAwait(false);

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });
            return Json(new { isSuccessful = false, errors });
        }
    }
}