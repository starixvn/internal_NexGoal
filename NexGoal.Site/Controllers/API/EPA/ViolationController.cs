namespace NexGoal.Site.Controllers.API
{
    using NexGoal.Core.Attribute;
    using NexGoal.Core.Constants;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Model;
    using NexGoal.Criteria.Violation;
    using NexGoal.Model.Violation;
    using NexGoal.Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Web.Http;

    public class ViolationController : BaseController
    {
        private readonly IViolationService violationService;

        public ViolationController()
            : this(null)
        {
        }

        public ViolationController(IViolationService integrationService)
        {
            this.violationService = integrationService ?? new ViolationService();
        }

        [HttpGet]
        public IEnumerable<ViolationViewModel> GetByPeriodId(int periodId, int employeeId)
        {
            var criteria = new GetViolationByPeriodIdCriteria { PeriodId = periodId, EmployeeId = employeeId };
            IEnumerable<ViolationViewModel> result = violationService.GetByPeriodId(criteria);

            return result;
        }

        [HttpGet]
        public int GetUserPermissionOnViolation()
        {
            return SessionService.CurrentUser.GetPermission(FunctionKey.ViolationManagement);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey.ViolationManagement, Policy.Read)]
        public ViolationViewModel GetById(int violationId)
        {
            ViolationViewModel result = violationService.GetById(violationId);

            return result;
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey.ViolationManagement, Policy.Read)]
        public IEnumerable<ViolationViewModel> Get(DateTime fromDate, DateTime toDate, string groupIds, int? isActive, int? employeeId)
        {
            var criteria = new GetViolationCriteria
            {
                FromDate = fromDate,
                ToDate = toDate,
                GroupIds = groupIds,
                IsActive = isActive,
                EmployeeId = employeeId
            };

            IEnumerable<ViolationViewModel> result = violationService.Get(criteria);

            return result;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey.ViolationManagement, Policy.Host)]
        public CommandResult Create(CreateViolationCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.CreatedBy = SessionService.EmployeeId;
            }

            CommandResult commandResult = violationService.Insert(criteria);
            return commandResult;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey.ViolationManagement, Policy.Host)]
        public CommandResult Update(UpdateViolationCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = SessionService.EmployeeId;
            }

            CommandResult commandResult = violationService.Update(criteria);
            return commandResult;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey.ViolationManagement, Policy.Host)]
        public CommandResult UpdateActive(UpdateViolationActiveCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = SessionService.EmployeeId;
            }

            CommandResult commandResult = violationService.UpdateActive(criteria);
            return commandResult;
        }
    }
}