namespace NexGoal.Core.Services
{
    using System.Collections.Generic;
    using Enumerations;
    using Models;

    public interface ISessionService
    {
        int EmployeeId { get; set; }
        string NickName { get; set; }
        string FullName { get; set; }
        IEnumerable<MenuItem> MenuItems { get; set; }
        IEnumerable<FunctionModel> Functions { get; set; }

        bool HasPermissionOnFunction(string functionKey, Policy policy);

        void ClearAll();
    }
}