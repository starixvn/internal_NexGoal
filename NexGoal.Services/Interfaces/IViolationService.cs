namespace NexGoal.Services.Interfaces
{
    using System.Collections.Generic;
    using NexGoal.Criteria.Violation;
    using NexGoal.Model.Violation;

    public interface IViolationService
    {
        ViolationViewModel GetById(int violationId);

        IEnumerable<ViolationViewModel> GetViolation(GetViolationCriteria criteria);

        IEnumerable<ViolationViewModel> GetByPeriodId(GetViolationByPeriodIdCriteria criteria);
    }
}