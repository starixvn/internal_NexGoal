namespace NexGoal.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using NexGoal.Models.Common;
    using NexGoal.Models.NominationPeriod;
    using NexGoal.Models.PerformanceRecord;

    public class EmployeeNominationRepository : IEmployeeNominationRepository
    {
        public async Task<IEnumerable<PerformanceRecordModel>> GetMemberNominationAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var nominations = await db.QueryAsync<PerformanceRecordModel, EmployeeBaseModel, EmployeeBaseModel, NominationPeriodModel, PerformanceRecordModel>(
                    (nomination, employee, creator, period) =>
                    {
                        nomination.Employees = new List<EmployeeBaseModel> { employee };
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