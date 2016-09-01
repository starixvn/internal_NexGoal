using Fanex.Data.Repository;
using NexGoal.Model.Employee;
using NexGoal.Model.PAFocus;
using System.Collections.Generic;

namespace NexGoal.Repositories.Interfaces
{
    public interface IPAFocusRepository
    {
        IEnumerable<PAFocusModel> GetPAFocus(ICriteria criteria);
    }
}
