namespace NexGoal.Site.Controllers.API
{
    using NexGoal.Core.Attribute;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Model;
    using NexGoal.Criteria.KPI;
    using NexGoal.Model.KPI;
    using NexGoal.Services.Implement;
    using NexGoal.Services.Interfaces;
    using System.Collections.Generic;
    using System.Web.Http;

    public class KPIController : BaseController
    {
        private readonly IKPIService _kpiService;

        public KPIController()
            : this(null)
        {
        }

        public KPIController(
            IKPIService kpiService)
        {
            _kpiService = kpiService ?? new KPIService();
        }

        [ApiAuthorize(FunctionKey.EPASetting, Policy.Read)]
        [HttpGet]
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

        [ApiAuthorize(FunctionKey.EPASetting, Policy.Host)]
        [HttpPost]
        public CommandResult CreateKPI(CreateKPICriteria criteria)
        {
            if (criteria != null)
            {
                criteria.CreatedBy = CurrentUser.EmployeeId;
            }

            CommandResult commandResult = _kpiService.CreateKPI(criteria);

            return commandResult;
        }

        [ApiAuthorize(FunctionKey.EPASetting, Policy.Host)]
        [HttpPut]
        public CommandResult UpdateKPI(UpdateKPICriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentUser.EmployeeId;
            }

            CommandResult commandResult = _kpiService.UpdateKPI(criteria);

            return commandResult;
        }

        [ApiAuthorize(FunctionKey.EPASetting, Policy.Host)]
        [HttpPut]
        public CommandResult UpdateStatusKPI(UpdateStatusKPICriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentUser.EmployeeId;
            }

            CommandResult commandResult = _kpiService.UpdateStatusKPI(criteria);

            return commandResult;
        }

        [ApiAuthorize(FunctionKey.EPASetting, Policy.Host)]
        [HttpPut]
        public CommandResult UpdateRequiredEvaluateKPI(UpdateRequiredEvaluateKPICriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentUser.EmployeeId;
            }

            CommandResult commandResult = _kpiService.UpdateRequiredEvaluateKPI(criteria);

            return commandResult;
        }

        [ApiAuthorize(FunctionKey.EPASetting, Policy.Host)]
        [HttpDelete]
        public CommandResult DeleteKPI(int kpiId)
        {
            var criteria = new DeleteKPICriteria
            {
                KPIId = kpiId,
                ModifiedBy = CurrentUser.EmployeeId
            };

            CommandResult commandResult = _kpiService.DeleteKPI(criteria);

            return commandResult;
        }

        public int GetUserPermissionOnKPI()
        {
            return CurrentUser.GetPermission(FunctionKey.EPASetting);
        }
    }
}