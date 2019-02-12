namespace NexGoal.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using NexGoal.Models.EmployeeNomination;

    public interface INominationReportRepository
    {
        Task<IEnumerable<NominationModel>> GetNominationAsync(ICriteria criteria);
    }
}