namespace NexGoal.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Web.Http;
    using NexGoal.Core.Attribute;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Model;
    using NexGoal.Criteria.PAFocus;
    using NexGoal.Model;
    using NexGoal.Model.PAFocus;
    using NexGoal.Services.Interfaces;
    using Services.Implement;

    public class PAFocusController : BaseController
    {
        private readonly IPAFocusService focusService;
        private readonly ICommonService commonService;

        public PAFocusController()
            : this(null, null)
        {
        }

        public PAFocusController(IPAFocusService focusService, ICommonService commonService)
        {
            this.focusService = focusService ?? new PAFocusService();
            this.commonService = commonService ?? new CommonService();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = FunctionKey.PAFocus, Policy = Policy.Read)]
        public IEnumerable<PAFocusProcessModel> GetPAFocusProcess(int periodId)
        {
            var result = this.focusService.GetPAFocusProcess(periodId);

            return result;
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = FunctionKey.Period, Policy = Policy.Read)]
        public IEnumerable<FocusProcessSettingModel> GetPAProcessSettingByPeriodId(int periodId)
        {
            var processSettings = this.focusService.GetPAFocusProcessSettingByPeriodId(periodId);

            return processSettings;
        }

        [ApiAuthorize(FunctionKey = FunctionKey.Period, Policy = Policy.Host)]
        [HttpPost]
        public CommandResult UpdatePAProcessSetting(UpdatePAFocusProcessSetingCriteria criteria)
        {
            var result = commonService.ExecuteNonQuery(criteria);

            return result;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = FunctionKey.PAFocus, Policy = Policy.Host)]
        public CommandResult UpdatePAFocus(IEnumerable<PAFocusModel> evaluations)
        {
            var result = this.focusService.UpdatePAFocus(SessionService.EmployeeId, evaluations);

            return result;
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = FunctionKey.PAFocus, Policy = Policy.Read)]
        public IEnumerable<PAFocusModel> GetPAFocus(int periodId, string groupIds = null, int? employeeId = null)
        {
            var criteria = new GetPAFocusCriteria
            {
                PeriodId = periodId,
                EmployeeId = employeeId,
                GroupIds = groupIds,
                EvaluatorId = SessionService.EmployeeId
            };
            var result = this.focusService.GetPAFocus(criteria);

            return result;
        }

        [HttpGet]
        public int GetPermission()
        {
            var permission = SessionService.CurrentUser.GetPermission(FunctionKey.PAFocus);

            return permission;
        }
    }
}