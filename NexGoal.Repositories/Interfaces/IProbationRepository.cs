namespace NexGoal.Repositories
{
    using Fanex.Data.Repository;
    using NexGoal.Models.Common;
    using NexGoal.Models.Probation;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IProbationRepository
    {
        Task<IEnumerable<ProbationEvaluationModel>> GetProbationEvaluationsAsync(ICriteria criteria);

        Task<IEnumerable<KPIGroupModel>> GetEvaluationDetailAsync(ICriteria criteria);

        Task<IEnumerable<ProbationerModel>> GetProbationersAsync(ICriteria criteria);

        Task<HRMModel> GetHRM(ICriteria criteria);

        Task<Tuple<IEnumerable<Group>, IEnumerable<Employee>>> GetGroupsAndEmployeesAsync(ICriteria criteria);
    }
}