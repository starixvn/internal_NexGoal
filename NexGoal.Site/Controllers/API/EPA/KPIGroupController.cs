namespace NexGoal.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Web.Http;
    using NexGoal.Core.Attribute;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Model;
    using NexGoal.Criteria.KPIGroup;
    using NexGoal.Model.KPIGroup;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;

    public class KPIGroupController : BaseController
    {
        private readonly IKPIGroupService _kpiGroupService;
        private readonly ICommonService commonService;

        public KPIGroupController()
            : this(null, null)
        {
        }

        public KPIGroupController(
            IKPIGroupService kpiGroupService,
            ICommonService commonService)
        {
            _kpiGroupService = kpiGroupService ?? new KPIGroupService();
            this.commonService = commonService ?? new CommonService();
        }

        [ApiAuthorize(FunctionKey = FunctionKey.EPASetting, Policy = Policy.Read)]
        [HttpGet]
        public IEnumerable<KPIGroupModel> GetKPIGroup(bool? isActive = null)
        {
            var criteria = new GetKPIGroupCriteria
            {
                IsActive = isActive
            };

            return _kpiGroupService.GetKPIGroup(criteria);
        }

        [ApiAuthorize(FunctionKey = FunctionKey.EPASetting, Policy = Policy.Read)]
        [HttpGet]
        public KPIGroupModel GetKPIGroupById(int kpiGroupId)
        {
            var criteria = new GetKPIGroupByIdCriteria
            {
                KPIGroupId = kpiGroupId
            };

            return _kpiGroupService.GetKPIGroupById(criteria);
        }

        [ApiAuthorize(FunctionKey = FunctionKey.EPASetting, Policy = Policy.Host)]
        [HttpPost]
        public CommandResult CreateKPIGroup(CreateKPIGroupCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.CreatedBy = SessionService.EmployeeId;
            }

            CommandResult commandResult = commonService.ExecuteNonQuery(criteria);

            return commandResult;
        }

        [ApiAuthorize(FunctionKey = FunctionKey.EPASetting, Policy = Policy.Host)]
        [HttpPut]
        public CommandResult UpdateKPIGroup(UpdateKPIGroupCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = SessionService.EmployeeId;
            }

            CommandResult commandResult = commonService.ExecuteNonQuery(criteria);

            return commandResult;
        }

        [ApiAuthorize(FunctionKey = FunctionKey.EPASetting, Policy = Policy.Host)]
        [HttpPut]
        public CommandResult UpdateStatusKPIGroup(UpdateStatusKPIGroupCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = SessionService.EmployeeId;
            }

            CommandResult commandResult = commonService.ExecuteNonQuery(criteria);

            return commandResult;
        }

        [ApiAuthorize(FunctionKey = FunctionKey.EPASetting, Policy = Policy.Host)]
        [HttpDelete]
        public CommandResult DeleteKPIGroup(int kpiGroupId)
        {
            var criteria = new DeleteKPIGroupCriteria
            {
                KPIGroupId = kpiGroupId,
                ModifiedBy = SessionService.EmployeeId
            };

            CommandResult commandResult = commonService.ExecuteNonQuery(criteria);

            return commandResult;
        }

        public int GetUserPermissionOnKPIGroup()
        {
            return SessionService.CurrentUser.GetPermission(FunctionKey.EPASetting);
        }
    }
}