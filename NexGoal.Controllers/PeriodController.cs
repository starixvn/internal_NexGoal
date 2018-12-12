namespace NexGoal.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Filters;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Models.PARecord;
    using NexGoal.Models.Period;
    using NexGoal.Services;
    using NexGoal.ViewModels.Period;

    [NexGoalPermission(FunctionKey = FunctionKey.EvaluationPeriod, Policy = Policy.Read)]
    public class PeriodController : Controller
    {
        private readonly IPeriodService periodService;
        private readonly ISessionService sessionService;

        public PeriodController(
            ISessionService sessionService,
            IPeriodService periodService)
        {
            this.sessionService = sessionService;
            this.periodService = periodService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(sessionService.HasPermissionOnFunction(FunctionKey.EvaluationPeriod, Policy.Host));
        }

        [HttpGet]
        public async Task<JsonResult> GetPeriods(bool? isActive = null)
        {
            IEnumerable<PeriodModel> periods = await periodService.GetPeriodAsync(isActive).ConfigureAwait(false);

            return Json(periods);
        }

        [HttpGet]
        public async Task<JsonResult> GetPeriodProcessSetting(int id)
        {
            IEnumerable<PeriodProcessSettingModel> processSettings = await periodService.GetPeriodProcessSettingAsync(id).ConfigureAwait(false);

            return Json(processSettings);
        }

        [HttpGet]
        public async Task<JsonResult> GetById(int id)
        {
            PeriodModel period = await periodService.GetByIdAsync(id).ConfigureAwait(false);

            return Json(period);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationPeriod, Policy = Policy.Host)]
        public async Task<JsonResult> Create(CreatePeriodViewModel model)
        {
            if (ModelState.IsValid)
            {
                var period = new CreatePeriodModel
                {
                    Name = model.Name,
                    TargetTypeId = model.TargetTypeId,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    DisciplineStartDate = model.DisciplineStartDate,
                    DisciplineEndDate = model.DisciplineEndDate,
                    FocusStartDate = model.FocusStartDate,
                    FocusEndDate = model.FocusEndDate,
                    Description = model.Description,
                    IsActive = model.IsActive
                };

                CommandResult commandResult = await periodService.CreateAsync(period).ConfigureAwait(false);

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });
            return Json(new { isSuccessful = false, errors });
        }

        [HttpGet]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationPeriod, Policy = Policy.Host)]
        public async Task<IActionResult> Edit(int id)
        {
            PeriodModel period = await periodService.GetByIdAsync(id).ConfigureAwait(false);

            if (period == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            return View(id);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationPeriod, Policy = Policy.Host)]
        public async Task<JsonResult> Edit(EditPeriodViewModel model)
        {
            if (ModelState.IsValid)
            {
                var period = new EditPeriodModel
                {
                    PeriodId = model.PeriodId,
                    Name = model.Name,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    DisciplineStartDate = model.DisciplineStartDate,
                    DisciplineEndDate = model.DisciplineEndDate,
                    FocusStartDate = model.FocusStartDate,
                    FocusEndDate = model.FocusEndDate,
                    Description = model.Description,
                    IsActive = model.IsActive
                };

                CommandResult commandResult = await periodService.EditAsync(period).ConfigureAwait(false);

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });
            return Json(new { isSuccessful = false, errors });
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationPeriod, Policy = Policy.Host)]
        public async Task<JsonResult> EditStatus(int periodId, bool isActive)
        {
            CommandResult commandResult = await periodService.EditStatusAsync(periodId, isActive).ConfigureAwait(false);

            return Json(commandResult);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationPeriod, Policy = Policy.Host)]
        public async Task<JsonResult> EditProcessSetting(int periodId, IEnumerable<EditProcessSettingModel> settings)
        {
            CommandResult commandResult = await periodService.EditProcessSettingAsync(periodId, settings).ConfigureAwait(false);

            return Json(commandResult);
        }

        [HttpGet]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationPeriod, Policy = Policy.Host)]
        public async Task<JsonResult> GetEmployeeSettings(int periodId, int? groupId = null, bool? isEvaluated = null, int? statusId = null)
        {
            IEnumerable<PARecordModel> result = await periodService.GetEmployeeSettingAsync(periodId, groupId, isEvaluated, statusId).ConfigureAwait(false);

            return Json(result);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationPeriod, Policy = Policy.Host)]
        public async Task<JsonResult> CreateEmployeeSettings(int periodId, string groupIds, int? employeeId = null)
        {
            CommandResult commandResult = await periodService.CreateEmployeeSettingAsync(periodId, groupIds, employeeId).ConfigureAwait(false);

            return Json(commandResult);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationPeriod, Policy = Policy.Host)]
        public async Task<JsonResult> EditEmployeeSetting(int periodId, int[] paRecordIds, IEnumerable<EditEmployeeSettingModel> processes)
        {
            CommandResult commandResult = await periodService.EditEmployeeSettingAsync(periodId, paRecordIds, processes).ConfigureAwait(false);

            return Json(commandResult);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationPeriod, Policy = Policy.Host)]
        public async Task<JsonResult> EditEmployeeSettingStatus(int paRecordId, bool isExcluded)
        {
            CommandResult commandResult = await periodService.EditEmployeeSettingStatusAsync(paRecordId, isExcluded).ConfigureAwait(false);

            return Json(commandResult);
        }

        [HttpGet]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationPeriod, Policy = Policy.Host)]
        public async Task<JsonResult> GetEmployeeSettingById(int paRecordId)
        {
            PARecordModel result = await periodService.GetEmployeeSettingByIdAsync(paRecordId).ConfigureAwait(false);

            return Json(result);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationPeriod, Policy = Policy.Host)]
        public async Task<JsonResult> GeneratePA(int periodId, string paRecordIds)
        {
            CommandResult commandResult = await periodService.GeneratePAAsync(periodId, paRecordIds, sessionService.EmployeeId).ConfigureAwait(false);

            return Json(commandResult);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationPeriod, Policy = Policy.Host)]
        public async Task<JsonResult> Delete(int periodId)
        {
            CommandResult commandResult = await periodService.DeleteAsync(periodId).ConfigureAwait(false);

            return Json(commandResult);
        }
    }
}