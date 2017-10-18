namespace NexGoal.Services.Interfaces
{
    using System.Collections.Generic;
    using Core.Constants;
    using Core.Enumerations;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Violation;
    using NexGoal.Model.Violation;
    using NexGoal.Repositories.Implement;
    using NexGoal.Repositories.Interfaces;

    public class ViolationService : BaseService, IViolationService
    {
        private readonly IViolationRepository violationRepository;
        private readonly ISessionService sessionService;

        public ViolationService()
            : this(null, null)
        {
        }

        public ViolationService(IViolationRepository violationRepository, ISessionService sessionService)
        {
            this.violationRepository = violationRepository ?? new ViolationRepository();
            this.sessionService = sessionService ?? new SessionService();
        }

        public ViolationViewModel GetById(int violationId)
        {
            var model = new ViolationViewModel();
            var criteria = new GetViolationByIdCriteria { ViolationId = violationId };
            if (criteria.IsValid())
            {
                model = this.violationRepository.GetById(criteria);
            }

            return model;
        }

        public IEnumerable<ViolationViewModel> GetViolation(GetViolationCriteria criteria)
        {
            IEnumerable<ViolationViewModel> model = new List<ViolationViewModel>();
            if (criteria != null && criteria.IsValid())
            {
                model = this.violationRepository.GetViolation(criteria);
            }

            return model;
        }

        public IEnumerable<ViolationViewModel> GetByPeriodId(GetViolationByPeriodIdCriteria criteria)
        {
            IEnumerable<ViolationViewModel> model = new List<ViolationViewModel>();
            var policyToView = CheckPermissionToView(criteria.EmployeeId);
            if (policyToView != Policy.None && criteria.IsValid())
            {
                model = violationRepository.GetViolation(criteria);
            }

            return model;
        }

        private Policy CheckPermissionToView(int employeeId)
        {
            if (employeeId != sessionService.EmployeeId)
            {
                var policy = sessionService.CurrentUser.GetPermission(FunctionKey.Evaluator) + sessionService.CurrentUser.GetPermission(FunctionKey.Reviewer);

                return policy > (int)Policy.None ? Policy.Read : Policy.None;
            }

            return Policy.Read;
        }
    }
}