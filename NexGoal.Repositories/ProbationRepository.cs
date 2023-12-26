namespace NexGoal.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using NexGoal.Models.Common;
    using NexGoal.Models.Probation;

    public class ProbationRepository : IProbationRepository
    {
        public async Task<IEnumerable<ProbationEvaluationModel>> GetProbationEvaluationsAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multi = await db.QueryMultipleAsync(criteria))
                {
                    var probationers = await multi.ReadAsync<ProbationEvaluationModel>();

                    var settings = await multi.ReadAsync<ProbationSettingModel>();

                    foreach (var probationer in probationers)
                    {
                        probationer.Settings = settings.Where(x => x.EmployeeId == probationer.EmployeeId);
                    }

                    return probationers;
                }
            }
        }

        public async Task<IEnumerable<KPIGroupModel>> GetEvaluationDetailAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multi = await db.QueryMultipleAsync(criteria))
                {
                    var groups = await multi.ReadAsync<KPIGroupModel>();

                    var details = await multi.ReadAsync<ProbationEvaluationDetailModel>();

                    foreach (var group in groups)
                    {
                        group.Details = details.Where(x => x.KPIGroupId == group.KPIGroupId);
                    }

                    return groups;
                }
            }
        }

        public async Task<IEnumerable<ProbationerModel>> GetProbationersAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multi = await db.QueryMultipleAsync(criteria))
                {
                    var probationers = await multi.ReadAsync<ProbationerModel>();

                    var settings = await multi.ReadAsync<ProbationSettingModel>();

                    foreach (var probationer in probationers)
                    {
                        probationer.Settings = settings.Where(x => x.EmployeeId == probationer.EmployeeId);
                    }

                    return probationers;
                }
            }
        }

        public async Task<HRMModel> GetHRM(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multi = await db.QueryMultipleAsync(criteria))
                {
                    var attendances = await multi.ReadAsync<AttendanceModel>();

                    var toeic = await multi.ReadFirstOrDefaultAsync<ToeicModel>();

                    return new HRMModel
                    {
                        Toeic = toeic,
                        Attendances = attendances
                    };
                }
            }
        }

        public async Task<ValueTuple<IEnumerable<Group>, IEnumerable<Employee>>> GetGroupsAndEmployeesAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multi = await db.QueryMultipleAsync(criteria))
                {
                    var groups = await multi.ReadAsync<Group>();

                    var employees = await multi.ReadAsync<Employee>();

                    return new (groups, employees);
                }
            }
        }
    }
}