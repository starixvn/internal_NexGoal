namespace NexGoal.Services
{
    using NexGoal.Models.Common;
    using NexGoal.Models.Probation;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IProbationReportService
    {
        Task<Tuple<IEnumerable<Group>, IEnumerable<Employee>>> GetGroupsAndEmployeesAsync(int actorId);

        Task<IEnumerable<ProbationReportModel>> GetAsync(GetProbationReportModel model);
    }
}