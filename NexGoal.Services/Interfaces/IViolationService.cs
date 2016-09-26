namespace NexGoal.Services.Interfaces
{
    using NexGoal.Core.Model;
    using NexGoal.Criteria.Violation;
    using NexGoal.Model.Violation;
    using System.Collections.Generic;

    public interface IViolationService
    {
        CommandResult Insert(CreateViolationCriteria criteria);

        CommandResult Update(UpdateViolationCriteria criteria);

        CommandResult UpdateActive(UpdateViolationActiveCriteria criteria);

        ViolationViewModel GetById(int violationId);

        IEnumerable<ViolationViewModel> Get(GetViolationCriteria criteria);

        IEnumerable<ViolationViewModel> GetByPeriodId(GetViolationByPeriodIdCriteria criteria);
    }
}