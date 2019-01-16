namespace NexGoal.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using NexGoal.Models.Common;
    using NexGoal.Models.PARecord;

    public class PARepository : IPARepository
    {
        public async Task<IEnumerable<PerformanceAppraisalResult>> GetPerformanceAppraisalResultAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var records = await db.QueryAsync<PerformanceAppraisalResult, PeriodBaseModel, PerformanceAppraisalResult>(
                    (pa, period) =>
                    {
                        pa.Period = period;
                        return pa;
                    },
                    "PeriodId",
                    criteria).ConfigureAwait(false);

                return records;
            }
        }

        public async Task<IEnumerable<PerformanceAppraisalResult>> GetMemberPerformanceAppraisalResultAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var records = await db.QueryAsync<PerformanceAppraisalResult, EmployeeBaseModel, PeriodBaseModel, PerformanceAppraisalResult>(
                    (pa, employee, period) =>
                    {
                        pa.Employee = employee;
                        pa.Period = period;
                        return pa;
                    },
                    "EmployeeId, PeriodId",
                    criteria).ConfigureAwait(false);

                return records;
            }
        }
    }
}