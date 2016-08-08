using NexGoal.Core.Model;
using System.Collections.Generic;

namespace NexGoal.Core.Services
{
    public interface ISessionService
    {
        int EmployeeId { get; set; }

        IEnumerable<FunctionModel> Functions { get; set; }

        UserSession CurrentUser { get; set; }
    }
}
