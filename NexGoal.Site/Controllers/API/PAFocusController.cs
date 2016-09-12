namespace NexGoal.Site.Controllers.API
{
    using NexGoal.Core.Attribute;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Model;
    using NexGoal.Criteria.PAFocus;
    using NexGoal.Model;
    using NexGoal.Model.PAFocus;
    using NexGoal.Services.Interfaces;
    using System.Collections.Generic;
    using System.Web.Http;

    public class PAFocusController : BaseController
    {
        private readonly IPAFocusService focusService;

        public PAFocusController()
            : this(null)
        {
        }

        public PAFocusController(IPAFocusService menuService)
        {
            this.focusService = menuService ?? new PAFocusService();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey.Period, Policy.Read)]
        public IEnumerable<FocusProcessSettingModel> GetPAProcessSettingByPeriodId(int periodId)
        {
            var processSettings = this.focusService.GetPAFocusProcessSettingByPeriodId(periodId);

            return processSettings;
        }

        [ApiAuthorize(FunctionKey.Period, Policy.Host)]
        [HttpPost]
        public CommandResult UpdatePAProcessSetting(UpdatePAFocusProcessSetingCriteria criteria)
        {
            var result = this.focusService.UpdateProcessSetting(criteria);

            return result;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey.PAFocus, Policy.Host)]
        public CommandResult UpdatePAFocus(IEnumerable<PAFocusModel> evaluations)
        {
            var result = this.focusService.UpdatePAFocus(SessionService.EmployeeId, evaluations);

            return result;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey.PAFocusFull, Policy.Host)]
        public CommandResult UpdatePAFocusByManagement(IEnumerable<PAFocusModel> evaluations)
        {
            var result = this.focusService.UpdatePAFocus(SessionService.EmployeeId, evaluations);

            return result;
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey.PAFocus, Policy.Read)]
        public IEnumerable<PAFocusModel> GetPAFocus(int periodId, string groupIds = null)
        {
            var criteria = new GetPAFocusCriteria
            {
                PeriodId = periodId,
                GroupIds = groupIds,
                EvaluatorId = SessionService.EmployeeId
            };
            var result = this.focusService.GetPAFocus(criteria);

            return result;
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey.PAFocusFull, Policy.Read)]
        public IEnumerable<PAFocusModel> GetPAFocusByManagement(int periodId, string groupIds = null, int? employeeId = null)
        {
            var criteria = new GetPAFocusByManagementCriteria
            {
                PeriodId = periodId,
                EmployeeId = employeeId,
                GroupIds = groupIds
            };
            var result = this.focusService.GetPAFocus(criteria);

            return result;
        }

        public int GetPAFocusFullPermission()
        {
            return SessionService.CurrentUser.GetPermission(FunctionKey.PAFocusFull);
        }
    }
}