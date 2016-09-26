using Fanex.Data.Repository;
using NexGoal.Model.Violation;
using System.Collections.Generic;

namespace NexGoal.Repositories.Interfaces
{
    public interface IViolationRepository
    {
        IEnumerable<ViolationViewModel> Get(ICriteria criteria);

        ViolationViewModel GetById(ICriteria criteria);
    }
}
