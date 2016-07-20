namespace NexGoal.Site.Controllers.API
{
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Model;
    using NexGoal.Criteria.KPIGroup;
    using NexGoal.Model.KPIGroup;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;
    using NexGoal.Web;
    using NexGoal.Web.ApiAuthorizeAttribute;
    using NexGoal.Web.Services;
    using System.Collections.Generic;
    using System.Web.Http;

    public class KPIGroupController : BaseController
    {
        private readonly IKPIGroupService _kpiGroupService;
        private readonly IWebCommonService _webCommonService;

        public KPIGroupController()
            : this(null, null)
        {
        }

        public KPIGroupController(
            IKPIGroupService kpiGroupService,
            IWebCommonService webCommonService)
        {
            _kpiGroupService = kpiGroupService ?? new KPIGroupService();
            _webCommonService = webCommonService ?? new WebCommonService();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.KPIGroup, Policy = Policy.Read)]
        public IEnumerable<KPIGroupModel> GetKPIGroup(bool? isActive = null)
        {
            var criteria = new GetKPIGroupCriteria
            {
                IsActive = isActive
            };

            return _kpiGroupService.GetKPIGroup(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.KPIGroup, Policy = Policy.Read)]
        public KPIGroupModel GetKPIGroupById(int kpiGroupId)
        {
            var criteria = new GetKPIGroupByIdCriteria
            {
                KPIGroupId = kpiGroupId
            };

            return _kpiGroupService.GetKPIGroupById(criteria);
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.KPIGroup, Policy = Policy.Host)]
        public CommandResult CreateKPIGroup(CreateKPIGroupCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.CreatedBy = SessionManager.CurrentUserId;
            }

            CommandResult commandResult = _kpiGroupService.CreateKPIGroup(criteria);

            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.KPIGroup, Policy = Policy.Host)]
        public CommandResult UpdateKPIGroup(UpdateKPIGroupCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = SessionManager.CurrentUserId;
            }

            CommandResult commandResult = _kpiGroupService.UpdateKPIGroup(criteria);

            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.KPIGroup, Policy = Policy.Host)]
        public CommandResult UpdateStatusKPIGroup(UpdateStatusKPIGroupCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = SessionManager.CurrentUserId;
            }

            CommandResult commandResult = _kpiGroupService.UpdateStatusKPIGroup(criteria);

            return commandResult;
        }

        [HttpDelete]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.KPIGroup, Policy = Policy.Host)]
        public CommandResult DeleteKPIGroup(int kpiGroupId)
        {
            var criteria = new DeleteKPIGroupCriteria
            {
                KPIGroupId = kpiGroupId,
                ModifiedBy = SessionManager.CurrentUserId
            };

            CommandResult commandResult = _kpiGroupService.DeleteKPIGroup(criteria);

            return commandResult;
        }

        public int GetUserPermissionOnKPIGroup()
        {
            int permission = _webCommonService.GetPermission(EPAFunctionKey.KPIGroup);
            return permission;
        }
    }
}