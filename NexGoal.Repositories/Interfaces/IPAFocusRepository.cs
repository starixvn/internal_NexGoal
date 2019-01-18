namespace NexGoal.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using NexGoal.Models.PAFocus;

    public interface IPAFocusRepository
    {
        Task<IEnumerable<PAFocusModel>> GetPAFocusAsync(ICriteria criteria);
    }
}