namespace NexGoal.Services.Interfaces
{
    using Core.Constants;
    using Core.Enumerations;
    using NexGoal.Core.Model;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.Violation;
    using NexGoal.Model.Violation;
    using NexGoal.Repositories.Implement;
    using NexGoal.Repositories.Interfaces;
    using System.Collections.Generic;

    public class ViolationService : BaseService, IViolationService
    {
        private readonly IViolationRepository violationRepository;
        private ISessionService sessionService;

        public ViolationService()
            : this(null, null)
        {
        }

        public ViolationService(IViolationRepository violationRepository, ISessionService sessionService)
        {
            this.violationRepository = violationRepository ?? new ViolationRepository();
            this.sessionService = sessionService ?? new SessionService();
        }

        public CommandResult Insert(CreateViolationCriteria criteria)
        {
            var result = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                result = Repository.Insert(criteria);
            }

            return result;
        }

        public CommandResult Update(UpdateViolationCriteria criteria)
        {
            var result = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                result = Repository.Update(criteria);
            }

            return result;
        }

        public CommandResult UpdateActive(UpdateViolationActiveCriteria criteria)
        {
            var result = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                result = Repository.Update(criteria);
            }

            return result;
        }

        public ViolationViewModel GetById(int violationId)
        {
            var model = new ViolationViewModel();
            var criteria = new GetViolationByIdCriteria { ViolationId = violationId };
            if (criteria != null && criteria.IsValid())
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
            if (policyToView != Policy.None)
            {
                if (criteria != null && criteria.IsValid())
                {
                    model = this.violationRepository.GetViolation(criteria);
                }
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