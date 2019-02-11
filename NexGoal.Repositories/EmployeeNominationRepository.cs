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

        public async Task<IEnumerable<PerformanceRecord>> GetMemberNominationAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var nominations = await db.QueryAsync<PerformanceRecord, Employee, Employee, NominationPeriodModel, PerformanceRecord>(
                    (nomination, nominee, creator, period) =>
                    {
                        nomination.Employees = new List<Employee> { nominee };
                        nomination.Creator = creator;
                        nomination.NominationPeriod = period;

                        return nomination;
                    },
                    "EmployeeId, EmployeeId, NominationPeriodId",
                    criteria).ConfigureAwait(false);

                return nominations;
            }
        }

        public async Task<PerformanceRecord> GetNominationByRecordIdAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return await db.QueryEntityAsync<PerformanceRecord, Employee, Employee, NominationPeriodModel, PerformanceRecord>(
                    (nomination, nominee, creator, period) =>
                    {
                        nomination.Employees = new List<Employee> { nominee };
                        nomination.Creator = creator;
                        nomination.NominationPeriod = period;

                        return nomination;
                    },
                    "EmployeeId, EmployeeId, NominationPeriodId",
                    criteria).ConfigureAwait(false);
            }
        }
    }
}