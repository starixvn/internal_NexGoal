namespace NexGoal.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Models.Redmine;

    public interface IRedmineService
    {
        Task<IList<EmployeeWorkLoadModel>> GetTeamWorkload(int EmployeeId, DateTime from, DateTime to);
    }
}