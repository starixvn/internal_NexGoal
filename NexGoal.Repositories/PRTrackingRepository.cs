namespace NexGoal.Repositories
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using NexGoal.Models.Common;
    using NexGoal.Models.Period;
    using NexGoal.Models.PRTracking;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class PRTrackingRepository : IPRTrackingRepository
    {
        public async Task<Tuple<PeriodModel, IEnumerable<PRTrackingModel>>> GetAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multi = await db.QueryMultipleAsync(criteria))
                {
                    var period = await multi.ReadFirstOrDefaultAsync<PeriodModel>();

                    var prTrackings = await multi.ReadAsync<PRTrackingModel>();

                    var details = await multi.ReadAsync<PRTrackingDetailModel>();

                    foreach (var prTracking in prTrackings)
                    {
                        prTracking.Details = details.Where(x => x.EmployeeId == prTracking.EmployeeId);
                    }

                    return new Tuple<PeriodModel, IEnumerable<PRTrackingModel>>(period, prTrackings);
                }
            }
        }

        public async Task<Tuple<IEnumerable<Group>, IEnumerable<Employee>>> GetGroupsAndMembersAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multi = await db.QueryMultipleAsync(criteria))
                {
                    var groups = await multi.ReadAsync<Group>();

                    var employees = await multi.ReadAsync<Employee>();

                    return new Tuple<IEnumerable<Group>, IEnumerable<Employee>>(groups, employees);
                }
            }
        }
    }
}