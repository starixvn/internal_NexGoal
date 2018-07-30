namespace NexGoal.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Filters;
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
        public async Task<IActionResult> GetDisciplines(DateTime from, DateTime to, bool? isActive, string groupIds, int? employeeId)
        {
            var disciplines = await disciplineService.GetDisciplines(from, to, isActive, groupIds, employeeId).ConfigureAwait(false);

            return Ok(disciplines);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.DisciplineManagement, Policy = Policy.Host)]
        public async Task<IActionResult> EditStatus(int disciplineId, bool isActive)
        {
            var result = await disciplineService.EditStatus(disciplineId, isActive, sessionService.EmployeeId).ConfigureAwait(false);

            return Json(result);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.DisciplineManagement, Policy = Policy.Host)]
        public async Task<IActionResult> Create(AddDisciplineViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await disciplineService.Create(new AddDisciplineModel
                {
                    EmployeeId = model.EmployeeId,
                    Content = model.Content,
                    EffectiveDate = model.EffectiveDate,
                    CreatedBy = sessionService.EmployeeId
                }).ConfigureAwait(false);

                return Json(result);
            }

            var errors = from item in ModelState
                         where item.Value.Errors.Any()
                         select new { item.Key, item.Value.Errors.FirstOrDefault().ErrorMessage };
            return Json(new { isSuccessful = false, errors });
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.DisciplineManagement, Policy = Policy.Host)]
        public async Task<IActionResult> Edit(EditDisciplineViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await disciplineService.Edit(new EditDisciplineModel
                {
                    DisciplineId = model.DisciplineId,
                    EffectiveDate = model.EffectiveDate,
                    Content = model.Content,
                    ModifiedBy = sessionService.EmployeeId
                }).ConfigureAwait(false);

                return Json(result);
            }

            var errors = from item in ModelState
                         where item.Value.Errors.Any()
                         select new { item.Key, item.Value.Errors.FirstOrDefault().ErrorMessage };
            return Json(new { isSuccessful = false, errors });
        }
    }
}