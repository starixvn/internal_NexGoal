namespace NexGoal.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using NexGoal.Models.EmployeeNomination;
    using NexGoal.Models.PerformanceRecord;

    public interface IEmployeeNominationRepository
    {
        Task<IEnumerable<EmployeeNominationModel>> GetNominationResultAsync(ICriteria criteria);

        Task<IEnumerable<PerformanceRecordModel>> GetMemberNominationAsync(ICriteria criteria);
    }
}