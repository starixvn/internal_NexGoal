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

        public PeriodController(IPeriodService periodService, ISessionService sessionService)
        {
            this.periodService = periodService;
            this.sessionService = sessionService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(sessionService.HasPermissionOnFunction(FunctionKey.EvaluationPeriod, Policy.Host));
        }

        [HttpGet]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationPeriod, Policy = Policy.Host)]
        public IActionResult Edit(int id)
        {
            return View(id);
        }

        [HttpGet]
        public async Task<JsonResult> GetPeriods(bool? isActive)
        {
            IEnumerable<PeriodModel> periods = await periodService.GetPeriods(isActive).ConfigureAwait(false);

            return Json(periods);
        }

        [HttpGet]
        public async Task<JsonResult> GetPeriodProcessSetting(int id)
        {
            IEnumerable<PeriodProcessSettingModel> processSettings = await periodService.GetPeriodProcessSetting(id).ConfigureAwait(false);

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

                CommandResult commandResult = await periodService.Create(period).ConfigureAwait(false);

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });
            return Json(new { isSuccessful = false, errors });
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationPeriod, Policy = Policy.Host)]
        public async Task<JsonResult> EditStatus(int periodId, bool isActive)
        {
            CommandResult commandResult = await periodService.EditStatus(periodId, isActive).ConfigureAwait(false);

            return Json(commandResult);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationPeriod, Policy = Policy.Host)]
        public async Task<JsonResult> Delete(int periodId)
        {
            CommandResult commandResult = await periodService.Delete(periodId).ConfigureAwait(false);

            return Json(commandResult);
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

                CommandResult commandResult = await periodService.Edit(period).ConfigureAwait(false);

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });
            return Json(new { isSuccessful = false, errors });
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationPeriod, Policy = Policy.Host)]
        public async Task<JsonResult> EditProcessSetting(int periodId, IEnumerable<PeriodProcessSettingModel> settings)
        {
            var processSettings = settings.Select(x => new EditProcessSettingModel
            {
                PeriodId = x.PeriodId,
                ProcessId = x.ProcessId,
                Duration = x.Duration,
                FocusDay = x.FocusDay,
                NoticeAtBegin = x.NoticeAtBegin,
                RemindDay = x.RemindDay
            });
            CommandResult commandResult = await periodService.EditProcessSetting(periodId, processSettings).ConfigureAwait(false);

            return Json(commandResult);
        }

        [HttpGet]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationPeriod, Policy = Policy.Host)]
        public async Task<JsonResult> GetEmployeeSettings(int periodId, int? groupId, bool? isEvaluated, int? statusId)
        {
            IEnumerable<PARecordModel> result = await periodService.GetEmployeeSettings(periodId, groupId, isEvaluated, statusId).ConfigureAwait(false);

            return Json(result);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationPeriod, Policy = Policy.Host)]
        public async Task<JsonResult> CreateEmployeeSettings(int periodId, string groupIds, int? employeeId)
        {
            CommandResult commandResult = await periodService.CreateEmployeeSettings(periodId, groupIds, employeeId).ConfigureAwait(false);

            return Json(commandResult);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationPeriod, Policy = Policy.Host)]
        public async Task<JsonResult> EditEmployeeSetting(int periodId, int[] paRecordIds, IEnumerable<EditEmployeeSettingViewModel> processes)
        {
            var serviceModels = processes.Select(x => new EditEmployeeSettingModel
            {
                ProcessId = x.ProcessId,
                EvaluatorId = x.EvaluatorId,
                StartDate = x.StartDate,
                EndDate = x.EndDate
            });

            CommandResult commandResult = await periodService.EditEmployeeSettings(periodId, paRecordIds, serviceModels).ConfigureAwait(false);

            return Json(commandResult);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationPeriod, Policy = Policy.Host)]
        public async Task<JsonResult> EditEmployeeSettingStatus(int paRecordId, int statusId)
        {
            CommandResult commandResult = await periodService.EditEmployeeSettingStatus(paRecordId, statusId).ConfigureAwait(false);

            return Json(commandResult);
        }

        [HttpGet]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationPeriod, Policy = Policy.Host)]
        public async Task<JsonResult> GetEmployeeSetting(int paRecordId)
        {
            PARecordModel result = await periodService.GetEmployeeSetting(paRecordId).ConfigureAwait(false);

            return Json(result);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationPeriod, Policy = Policy.Host)]
        public async Task<JsonResult> GeneratePA(int periodId, string paRecordIds)
        {
            CommandResult commandResult = await periodService.GeneratePA(periodId, paRecordIds, sessionService.EmployeeId).ConfigureAwait(false);

            return Json(commandResult);
        }
    }
}