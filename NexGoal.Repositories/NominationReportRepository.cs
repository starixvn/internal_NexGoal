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

    public class NominationReportRepository : INominationReportRepository
    {
        public async Task<IEnumerable<NominationModel>> GetNominationAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = await db.QueryMultipleAsync(criteria).ConfigureAwait(false))
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