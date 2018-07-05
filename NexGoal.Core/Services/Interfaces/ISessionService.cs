namespace NexGoal.Core.Services
{
    using System.Collections.Generic;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Models;

    public interface ISessionService
    {
        int EmployeeId { get; set; }
        string NickName { get; set; }
        string FullName { get; set; }
        IEnumerable<MenuItemSession> MenuItems { get; set; }
        IEnumerable<FunctionSession> Functions { get; set; }

        bool HasPermissionOnFunction(string functionKey, Policy policy);

        void ClearAll();
    }
}