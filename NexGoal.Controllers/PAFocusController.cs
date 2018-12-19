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
    using NexGoal.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.PAFocus;
    using NexGoal.Services;
    using NexGoal.ViewModels.PAFocus;

    [NexGoalPermission(FunctionKey = FunctionKey.PAFocus, Policy = Policy.Read)]
    public class PAFocusController : Controller
    {
        private readonly ISessionService sessionService;
        private readonly ICommonService commonService;
        private readonly IPAFocusService focusService;
        private readonly IExportPAFocusService exportExcelService;

        public PAFocusController(
            ISessionService sessionService,
            ICommonService commonService,
            IPAFocusService focusService,
            IExportPAFocusService exportExcelService)
        {
            this.sessionService = sessionService;
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
            IEnumerable<ReferenceResourceModel> referenceResources = await commonService.GetReferenceResourcesByNamesAsync(refNames).ConfigureAwait(false);
            IEnumerable<FocusEvaluationLevelModel> levels =
                referenceResources.Select(x => new FocusEvaluationLevelModel { Value = x.ResourceId, Name = x.Value })
                .OrderByDescending(x => x.Value);

            return Json(levels);
        }

        public async Task<JsonResult> GetPeriod()
        {
            IEnumerable<PeriodBaseModel> periods = await focusService.GetPeriodAsync().ConfigureAwait(false);

            return Json(periods);
        }

        public async Task<JsonResult> GetGroup()
        {
            IEnumerable<GroupBaseModel> groups = await focusService.GetGroupAsync(sessionService.EmployeeId).ConfigureAwait(false);

            return Json(groups);
        }

        public async Task<JsonResult> GetMember(string groupIds = null)
        {
            IEnumerable<EmployeeBaseModel> members = await focusService.GetMemberAsync(sessionService.EmployeeId, groupIds).ConfigureAwait(false);

            return Json(members);
        }

        public async Task<JsonResult> GetPAFocus(GetPAFocusParam param)
        {
            param.ActorId = sessionService.EmployeeId;
            IEnumerable<PAFocusModel> paFocuses = await focusService.GetPAFocusAsync(param).ConfigureAwait(false);
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
            CommandResult commandResult = await focusService.EditPAFocusAsync(sessionService.EmployeeId, paFocuses).ConfigureAwait(false);

            return Json(commandResult);
        }

        public async Task<IActionResult> ExportToExcel(GetPAFocusParam param)
        {
            param.ActorId = sessionService.EmployeeId;
            IEnumerable<PeriodBaseModel> periods = await focusService.GetPeriodAsync().ConfigureAwait(false);
            IEnumerable<PAFocusModel> paFocuses = await focusService.GetPAFocusAsync(param).ConfigureAwait(false);
            PeriodBaseModel period = periods.First(x => x.PeriodId == param.PeriodId);
            var fileName = string.Format("PA Focus of {0}.xlsx", period != null ? period.Name : string.Empty);

            byte[] fileContent = exportExcelService.Export(period, paFocuses);

            return File(fileContent, MimeType.XLSX, fileName);
        }
    }
}