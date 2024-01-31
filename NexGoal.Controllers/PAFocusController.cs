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
    using NexGoal.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.PAFocus;
    using NexGoal.Services;
    using NexGoal.ViewModels.PAFocus;
    using NexGoal.Web.Core.Filters;
    using NexGoal.Web.Core.Services;

    [NexGoalPermission(FunctionKey = FunctionKey.PAFocus, Policy = Policy.Read)]
    public class PAFocusController : BaseController
    {
        private readonly ICommonService commonService;
        private readonly IPAFocusService focusService;
        private readonly IExportPAFocusService exportExcelService;

        public PAFocusController(
            ISessionService sessionService,
            ICommonService commonService,
            IPAFocusService focusService,
            IExportPAFocusService exportExcelService) : base(sessionService)
        {
            this.commonService = commonService;
            this.focusService = focusService;
            this.exportExcelService = exportExcelService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<JsonResult> GetFocusEvaluationLevel()
        {
            string[] refNames = { ReferenceResource.FocusEvaluationLevel };
            IEnumerable<ReferenceResourceModel> referenceResources = await commonService.GetReferenceResourcesByNamesAsync(refNames);
            IEnumerable<FocusEvaluationLevelModel> levels =
                referenceResources.Select(x => new FocusEvaluationLevelModel { Value = x.ResourceId, Name = x.Value })
                .OrderByDescending(x => x.Value);

            return Json(levels);
        }

        public async Task<JsonResult> GetPeriod()
        {
            IEnumerable<PeriodBaseModel> periods = await focusService.GetPeriodAsync();

            return Json(periods);
        }

        public async Task<JsonResult> GetLastPerformanceRecordByEmployee(int employeeId, DateTime fromDate, DateTime toDate)
        {
            var performanceRecord = await focusService.GetLastPerformanceRecordByEmployeeAsync(employeeId, fromDate, toDate);

            return Json(performanceRecord);
        }

        public async Task<JsonResult> GetGroup()
        {
            IEnumerable<Group> groups = await focusService.GetGroupAsync(sessionService.EmployeeId);

            return Json(groups);
        }

        public async Task<JsonResult> GetMember(string groupIds = null)
        {
            IEnumerable<Employee> members = await focusService.GetMemberAsync(sessionService.EmployeeId, groupIds);

            return Json(members);
        }

        public async Task<JsonResult> GetPAFocus(GetPAFocusParam param)
        {
            param.ActorId = sessionService.EmployeeId;
            IEnumerable<PAFocusModel> paFocuses = await focusService.GetPAFocusAsync(param);
            bool isHost = paFocuses.Any(x => x.IsHost) && sessionService.HasPermissionOnFunction(FunctionKey.PAFocus, Policy.Host);

            IEnumerable<PAFocusViewModel> result = paFocuses.GroupBy(x => new { x.Employee.GroupCode, x.Employee.GroupName },
                (key, g) => new PAFocusViewModel
                {
                    GroupCode = key.GroupCode,
                    GroupName = key.GroupName,
                    Members = g
                });

            return Json(new { isHost, paFocuses = result });
        }

        [HttpPost]
        [NexGoalPermission(FunctionKey = FunctionKey.PAFocus, Policy = Policy.Host)]
        public async Task<JsonResult> Edit(IEnumerable<EditPAFocusModel> paFocuses)
        {
            CommandResult commandResult = await focusService.EditPAFocusAsync(sessionService.EmployeeId, paFocuses);

            return Json(commandResult);
        }

        public async Task<IActionResult> ExportToExcel(GetPAFocusParam param)
        {
            param.ActorId = sessionService.EmployeeId;
            IEnumerable<PeriodBaseModel> periods = await focusService.GetPeriodAsync();
            IEnumerable<PAFocusModel> paFocuses = await focusService.GetPAFocusAsync(param);
            PeriodBaseModel period = periods.First(x => x.PeriodId == param.PeriodId);
            var fileName = string.Format("PA Focus of {0}.xlsx", period != null ? period.Name : string.Empty);

            byte[] fileContent = await exportExcelService.ExportAsync(period, paFocuses);

            return File(fileContent, MimeType.XLSX, fileName);
        }
    }
}