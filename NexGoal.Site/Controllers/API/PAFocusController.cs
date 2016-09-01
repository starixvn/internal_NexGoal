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

        [HttpPost]
        //[ApiAuthorize(FunctionKey.Period, Policy.Host)]
        public CommandResult UpdatePAProcessSetting(int periodId, IEnumerable<FocusProcessSettingModel> process)
        {
            var result = this.focusService.UpdateProcessSetting(periodId, process);

            return result;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey.PAFocus, Policy.Host)]
        public CommandResult UpdatePAFocus(IEnumerable<PAFocusModel> evaluations)
        {
            var result = this.focusService.UpdatePAFocus(SessionService.EmployeeId, evaluations);

            return result;
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey.Evaluator, Policy.Read)]
        public IEnumerable<PAFocusModel> GetPAFocusByEvaluator(string periodIds = null, string groupIds = null, int? employeeId = null)
        {
            var criteria = new GetPAFocusByEvaluatorCriteria
            {
                PeriodIds = periodIds,
                EmployeeId = employeeId,
                GroupIds = groupIds,
                EvaluatorId = SessionService.EmployeeId
            };
            var result = this.focusService.GetPAFocus(criteria);

            return result;
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey.Reviewer, Policy.Read)]
        public IEnumerable<PAFocusModel> GetPAFocusByReviwer(string periodIds = null, string groupIds = null, int? employeeId = null)
        {
            var criteria = new GetPAFocusByReviewerCriteria
            {
                PeriodIds = periodIds,
                EmployeeId = employeeId,
                GroupIds = groupIds,
                ReviewerId = SessionService.EmployeeId
            };
            var result = this.focusService.GetPAFocus(criteria);

            return result;
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey.Management, Policy.Read)]
        public IEnumerable<PAFocusModel> GetPAFocusByManagement(string periodIds = null, string groupIds = null, int? employeeId = null)
        {
            var criteria = new GetPAFocusByManagementCriteria
            {
                PeriodIds = periodIds,
                EmployeeId = employeeId,
                GroupIds = groupIds,
                ManagementId = SessionService.EmployeeId
            };
            var result = this.focusService.GetPAFocus(criteria);

            return result;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey.Evaluator, Policy.Host)]
        public CommandResult UpdatePAFocusByEvaluator(IEnumerable<PAFocusModel> evaluations)
        {
            var result = this.focusService.UpdatePAFocus(SessionService.EmployeeId, evaluations);

            return result;
        }
    }
}