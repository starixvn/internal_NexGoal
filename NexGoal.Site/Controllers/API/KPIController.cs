namespace NexGoal.Site.Controllers.API
{
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Model;
    using NexGoal.Criteria.KPI;
    using NexGoal.Model.KPI;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;
    using NexGoal.Web;
    using NexGoal.Web.ApiAuthorizeAttribute;
    using NexGoal.Web.Services;
    using System.Collections.Generic;
    using System.Web.Http;

    public class KPIController : BaseController
    {
        private readonly IKPIService _kpiService;
        private readonly IWebCommonService _webCommonService;

        public KPIController()
            : this(null, null)
        {
        }

        public KPIController(
            IKPIService kpiService,
            IWebCommonService webCommonService)
        {
            _kpiService = kpiService ?? new KPIService();
            _webCommonService = webCommonService ?? new WebCommonService();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.KPI, Policy = Policy.Read)]
        public IEnumerable<KPIModel> GetKPI(
            int? kpiGroupId = null,
            bool? isActive = null,
            bool? isRequiredEvaluate = null)
        {
            var criteria = new GetKPICriteria
            {
                KPIGroupId = kpiGroupId,
                IsActive = isActive,
                IsRequiredEvaluate = isRequiredEvaluate
            };

            return _kpiService.GetKPI(criteria);
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.KPI, Policy = Policy.Host)]
        public CommandResult CreateKPI(CreateKPICriteria criteria)
        {
            if (criteria != null)
            {
                criteria.CreatedBy = SessionManager.CurrentUserId;
            }

            CommandResult commandResult = _kpiService.CreateKPI(criteria);

            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.KPI, Policy = Policy.Host)]
        public CommandResult UpdateKPI(UpdateKPICriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = SessionManager.CurrentUserId;
            }

            CommandResult commandResult = _kpiService.UpdateKPI(criteria);

            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.KPI, Policy = Policy.Host)]
        public CommandResult UpdateStatusKPI(UpdateStatusKPICriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = SessionManager.CurrentUserId;
            }

            CommandResult commandResult = _kpiService.UpdateStatusKPI(criteria);

            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.KPI, Policy = Policy.Host)]
        public CommandResult UpdateRequiredEvaluateKPI(UpdateRequiredEvaluateKPICriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = SessionManager.CurrentUserId;
            }

            CommandResult commandResult = _kpiService.UpdateRequiredEvaluateKPI(criteria);

            return commandResult;
        }

        [HttpDelete]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.KPI, Policy = Policy.Host)]
        public CommandResult DeleteKPI(int kpiId)
        {
            var criteria = new DeleteKPICriteria
            {
                KPIId = kpiId,
                ModifiedBy = SessionManager.CurrentUserId
            };

            CommandResult commandResult = _kpiService.DeleteKPI(criteria);

            return commandResult;
        }

        public int GetUserPermissionOnKPI()
        {
            int permission = _webCommonService.GetPermission(EPAFunctionKey.KPI);
            return permission;
        }
    }
}