namespace NexGoal.Repositories
{
    using Fanex.Data.Repository;
    using NexGoal.Models.Probation;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IProbationRepository
    {
        Task<IEnumerable<ProbationEvaluationModel>> GetProbationersAsync(ICriteria criteria);
    }
}