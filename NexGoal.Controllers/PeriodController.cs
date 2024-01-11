namespace NexGoal.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;
    using NexGoal.Models.PARecord;
    using NexGoal.Models.Period;
    using NexGoal.Services;
    using NexGoal.ViewModels.Period;
    using NexGoal.Web.Core.Filters;
    using NexGoal.Web.Core.Services;

    public class PeriodController : BaseController
    {
        private readonly IPeriodService periodService;

        public PeriodController(
            ISessionService sessionService,
            IPeriodService periodService) : base(sessionService)
        {
            this.periodService = periodService;
        }

        [HttpGet]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationPeriod, Policy = Policy.Read)]
        public IActionResult Index()
        {
            return View(sessionService.HasPermissionOnFunction(FunctionKey.EvaluationPeriod, Policy.Host));
        }

        [HttpGet]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationPeriod, Policy = Policy.Read)]
        public async Task<JsonResult> GetPeriods(bool? isActive = null)
        {
            IEnumerable<PeriodModel> periods = await periodService.GetPeriodAsync(isActive);

            return Json(periods);
        }

        [HttpGet]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationPeriod, Policy = Policy.Read)]
        public async Task<JsonResult> GetPeriodProcessSetting(int id)
        {
            IEnumerable<PeriodProcessSettingModel> processSettings = await periodService.GetPeriodProcessSettingAsync(id);

            return Json(processSettings);
        }

        [HttpGet]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationPeriod, Policy = Policy.Read)]
        public async Task<JsonResult> GetById(int id)
        {
            PeriodModel period = await periodService.GetByIdAsync(id);

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

                CommandResult commandResult = await periodService.CreateAsync(sessionService.EmployeeId, period);

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors[0].ErrorMessage });
            return Json(new { isSuccessful = false, errors });
        }

        [HttpGet]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationPeriod, Policy = Policy.Host)]
        public async Task<IActionResult> Edit(int id)
        {
            PeriodModel period = await periodService.GetByIdAsync(id);

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

                CommandResult commandResult = await periodService.EditAsync(sessionService.EmployeeId, period);

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors[0].ErrorMessage });
            return Json(new { isSuccessful = false, errors });
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationPeriod, Policy = Policy.Host)]
        public async Task<JsonResult> EditStatus(int periodId, bool isActive)
        {
            CommandResult commandResult = await periodService.EditStatusAsync(sessionService.EmployeeId, periodId, isActive);

            return Json(commandResult);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationPeriod, Policy = Policy.Host)]
        public async Task<JsonResult> EditProcessSetting(int periodId, IEnumerable<EditProcessSettingModel> settings)
        {
            CommandResult commandResult = await periodService.EditProcessSettingAsync(sessionService.EmployeeId, periodId, settings);

            return Json(commandResult);
        }

        [HttpGet]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationPeriod, Policy = Policy.Host)]
        public async Task<JsonResult> GetEmployeeSettings(int periodId, int? groupId = null, bool? isEvaluated = null, int? statusId = null)
        {
            IEnumerable<PARecordModel> result = await periodService.GetEmployeeSettingAsync(periodId, groupId, isEvaluated, statusId);

            return Json(result);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationPeriod, Policy = Policy.Host)]
        public async Task<JsonResult> CreateEmployeeSettings(int periodId, string groupIds, int? employeeId = null)
        {
            CommandResult commandResult = await periodService.CreateEmployeeSettingAsync(sessionService.EmployeeId, periodId, groupIds, employeeId);

            return Json(commandResult);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationPeriod, Policy = Policy.Host)]
        public async Task<JsonResult> EditEmployeeSetting(int periodId, int[] paRecordIds, IEnumerable<EditEmployeeSettingModel> processes)
        {
            CommandResult commandResult = await periodService.EditEmployeeSettingAsync(sessionService.EmployeeId, periodId, paRecordIds, processes);

            return Json(commandResult);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationPeriod, Policy = Policy.Host)]
        public async Task<JsonResult> EditEmployeeSettingStatus(int paRecordId, bool isExcluded)
        {
            CommandResult commandResult = await periodService.EditEmployeeSettingStatusAsync(sessionService.EmployeeId, paRecordId, isExcluded);

            return Json(commandResult);
        }

        [HttpGet]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationPeriod, Policy = Policy.Host)]
        public async Task<JsonResult> GetEmployeeSettingById(int paRecordId)
        {
            PARecordModel result = await periodService.GetEmployeeSettingByIdAsync(paRecordId);

            return Json(result);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationPeriod, Policy = Policy.Host)]
        public async Task<JsonResult> GeneratePA(int periodId, string paRecordIds)
        {
            CommandResult commandResult = await periodService.GeneratePAAsync(periodId, paRecordIds, sessionService.EmployeeId);

            return Json(commandResult);
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.EvaluationPeriod, Policy = Policy.Host)]
        public async Task<JsonResult> Delete(int periodId)
        {
            CommandResult commandResult = await periodService.DeleteAsync(sessionService.EmployeeId, periodId);

            return Json(commandResult);
        }

        [Route("admin/period/updatePA/{periodId:int:min(1)}/{employeeId?}")]
        public async Task<IActionResult> UpdatePA(int periodId, int? employeeId = null)
        {
            IEnumerable<PARecordModel> records = await periodService.GetEmployeeSettingAsync(periodId);
            string ids;
            if (employeeId.HasValue)
            {
                var record = records.FirstOrDefault(x => x.EmployeeId == employeeId.Value);
                ids = record != null ? record.PARecordId.ToString() : string.Empty;
            }
            else
            {
                ids = string.Join(",", records.Select(x => x.PARecordId));
            }

            CommandResult commandResult = await periodService.EditPAGradeAsync(ids);
            return Ok(commandResult.IsSuccessful ? "Successfully!" : commandResult.Message);
        }
    }
}