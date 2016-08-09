using Fanex.Data.Repository;
using NexGoal.Model.Integration;
using System.Collections.Generic;

namespace NexGoal.Repositories.Interfaces
{
    public interface IIntegrationRepository
    {
        IEnumerable<EmployeeWorkingViewModel> GetEmployeeWorking(ICriteria criteria);
    }
}
