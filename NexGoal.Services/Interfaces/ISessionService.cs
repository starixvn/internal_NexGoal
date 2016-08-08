using NexGoal.Core.Model;
using System.Collections.Generic;

namespace NexGoal.Services.Interfaces
{
    public interface ISessionService
    {
        int EmployeeId { get; set; }

        IEnumerable<FunctionModel> Functions { get; set; }

        UserSession CurrentUser { get; set; }
    }
}
