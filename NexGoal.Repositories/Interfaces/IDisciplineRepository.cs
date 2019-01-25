namespace NexGoal.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using NexGoal.Models.Discipline;

    public interface IDisciplineRepository
    {
        Task<IEnumerable<DisciplineModel>> GetDisciplineAsync(ICriteria criteria);
    }
}