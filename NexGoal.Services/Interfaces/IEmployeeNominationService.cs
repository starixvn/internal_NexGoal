namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Models.EmployeeNomination;

    public interface IEmployeeNominationService
    {
        Task<IEnumerable<NominationPeriodModel>> GetNominationPeriodsForNominator(int nominatorId);
    }
}