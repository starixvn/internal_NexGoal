namespace NexGoal.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
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
                    criteria);

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
                    criteria);

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
                    criteria);
            }
        }

        public async Task<IEnumerable<NominationModel>> GetNominationByPeriodIdsAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = await db.QueryMultipleAsync(criteria))
                {
                    var nominations = multi.Read<NominationModel, Employee, NominationPeriodModel, NominationModel>(
                        (nomination, nominee, period) =>
                        {
                            nomination.Nominee = nominee;
                            nomination.Period = period;

                            return nomination;
                        },
                         "EmployeeId, NominationPeriodId"
                        );

                    var performanceRecords = multi.Read<PerformanceRecord, Employee, Employee, NominationPeriodModel, PerformanceRecord>(
                        (record, employee, creator, nominationPeriod) =>
                        {
                            record.Employees = new List<Employee> { employee };
                            record.Creator = creator;
                            record.NominationPeriod = nominationPeriod;

                            return record;
                        },
                        "EmployeeId, EmployeeId, NominationPeriodId"
                        );

                    foreach (NominationModel nomination in nominations)
                    {
                        nomination.Nominators = performanceRecords
                            .Where(x => x.Employees.First().EmployeeId == nomination.Nominee.EmployeeId && x.NominationPeriod.NominationPeriodId == nomination.Period.NominationPeriodId);
                    }

                    return nominations;
                }
            }
        }
    }
}