namespace NexGoal.Services.Interfaces
{
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

        public ViolationService()
            : this(null)
        {
        }

        public ViolationService(IViolationRepository violationRepository)
        {
            this.violationRepository = violationRepository ?? new ViolationRepository();
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

        public IEnumerable<ViolationViewModel> Get(GetViolationCriteria criteria)
        {
            IEnumerable<ViolationViewModel> model = new List<ViolationViewModel>();
            if (criteria != null && criteria.IsValid())
            {
                model = this.violationRepository.Get(criteria);
            }

            return model;
        }

        public IEnumerable<ViolationViewModel> GetByPeriodId(GetViolationByPeriodIdCriteria criteria)
        {
            IEnumerable<ViolationViewModel> model = new List<ViolationViewModel>();
            if (criteria != null && criteria.IsValid())
            {
                model = this.violationRepository.GetByPeriodId(criteria);
            }

            return model;
        }
    }
}