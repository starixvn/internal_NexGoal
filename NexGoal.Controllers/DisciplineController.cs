namespace NexGoal.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Models.Discipline;
    using NexGoal.Services;
    using NexGoal.ViewModels.Discipline;
    using NexGoal.Web.Core.Filters;
    using NexGoal.Web.Core.Services;

    [NexGoalPermission(FunctionKey = FunctionKey.Discipline, Policy = Policy.Read)]
    public class DisciplineController : BaseController
    {
        private readonly IDisciplineService disciplineService;

        public DisciplineController(
            IDisciplineService disciplineService,
            ISessionService sessionService) : base(sessionService)
        {
            this.disciplineService = disciplineService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(sessionService.HasPermissionOnFunction(FunctionKey.Discipline, Policy.Host));
        }

        [HttpGet]
        public async Task<JsonResult> GetDisciplines(GetDisciplineParam param)
        {
            IEnumerable<DisciplineModel> disciplines = await disciplineService.GetDisciplineAsync(param);

            return Json(disciplines);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.Discipline, Policy = Policy.Host)]
        public async Task<JsonResult> EditStatus(int disciplineId, bool isActive)
        {
            CommandResult commandResult = await disciplineService.EditStatusAsync(disciplineId, isActive, sessionService.EmployeeId);

            return Json(commandResult);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.Discipline, Policy = Policy.Host)]
        public async Task<JsonResult> Create(AddDisciplineViewModel model)
        {
            if (ModelState.IsValid)
            {
                CommandResult commandResult = await disciplineService.CreateAsync(new AddDisciplineModel
                {
                    EmployeeId = model.EmployeeId,
                    Content = model.Content,
                    EffectiveDate = model.EffectiveDate,
                    CreatedBy = sessionService.EmployeeId
                });

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });
            return Json(new { isSuccessful = false, errors });
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.Discipline, Policy = Policy.Host)]
        public async Task<JsonResult> Edit(EditDisciplineViewModel model)
        {
            if (ModelState.IsValid)
            {
                CommandResult commandResult = await disciplineService.EditAsync(new EditDisciplineModel
                {
                    DisciplineId = model.DisciplineId,
                    EffectiveDate = model.EffectiveDate,
                    Content = model.Content,
                    ModifiedBy = sessionService.EmployeeId
                });

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });
            return Json(new { isSuccessful = false, errors });
        }
    }
}