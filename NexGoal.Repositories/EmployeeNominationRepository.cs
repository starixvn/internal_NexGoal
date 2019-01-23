namespace NexGoal.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using NexGoal.Models.Common;
    using NexGoal.Models.EmployeeNomination;
    using NexGoal.Models.NominationPeriod;
    using NexGoal.Models.PerformanceRecord;

    public class EmployeeNominationRepository : IEmployeeNominationRepository
    {
        public async Task<IEnumerable<EmployeeNominationModel>> GetNominationResultAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var nominations = await db.QueryAsync<EmployeeNominationModel, NominationPeriodModel, EmployeeNominationModel>(
                    (nomination, period) =>
                    {
                        nomination.Period = period;

                        return nomination;
                    },
                    "NominationPeriodId",
                    criteria).ConfigureAwait(false);

                return nominations;
            }
        }

        public async Task<IEnumerable<PerformanceRecordModel>> GetMemberNominationAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var nominations = await db.QueryAsync<PerformanceRecordModel, Employee, Employee, NominationPeriodModel, PerformanceRecordModel>(
                    (nomination, employee, creator, period) =>
                    {
                        nomination.Employees = new List<Employee> { employee };
                        nomination.Creator = creator;
                        nomination.NominationPeriod = period;

                        return nomination;
                    },
                    "EmployeeId, EmployeeId, NominationPeriodId",
                    criteria).ConfigureAwait(false);

                return nominations;
            }
        }
    }
}