namespace NexGoal.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using NexGoal.Models.Common;
    using NexGoal.Models.PARecord;
    using NexGoal.Models.PerformanceRecord;

    public class PARepository : IPARepository
    {
        public async Task<IEnumerable<PerformanceAppraisalResult>> GetPerformanceAppraisalResultAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var performanceAppraisals = await db.QueryAsync<PerformanceAppraisalResult, PeriodBaseModel, PerformanceAppraisalResult>(
                    (pa, period) =>
                    {
                        pa.Period = period;
                        return pa;
                    },
                    "PeriodId",
                    criteria).ConfigureAwait(false);

                return performanceAppraisals;
            }
        }

        public async Task<IEnumerable<PerformanceAppraisalResult>> GetMemberPerformanceAppraisalResultAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var performanceAppraisals = await db.QueryAsync<PerformanceAppraisalResult, Employee, PeriodBaseModel, PerformanceAppraisalResult>(
                    (pa, employee, period) =>
                    {
                        pa.Employee = employee;
                        pa.Period = period;
                        return pa;
                    },
                    "EmployeeId, PeriodId",
                    criteria).ConfigureAwait(false);

                return performanceAppraisals;
            }
        }

        public async Task<PARecordModel> GetPARecordByIdAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = await db.QueryMultipleAsync(criteria).ConfigureAwait(false))
                {
                    var performanceAppraisals = multi.Read<PARecordModel, Employee, PeriodBaseModel, PARecordModel>(
                        (pa, employee, period) =>
                        {
                            pa.Employee = employee;
                            pa.Period = period;
                            return pa;
                        },
                        "EmployeeId, PeriodId");

                    var performanceAppraisal = performanceAppraisals.FirstOrDefault();

                    if (performanceAppraisal != null)
                    {
                        performanceAppraisal.Processes = multi.Read<PAProcessModel, Employee, PerformanceRecord, Employee, PAProcessModel>(
                            (process, evaluator, performance, creator) =>
                            {
                                if (performance != null)
                                {
                                    performance.Creator = creator;
                                }
                                process.Evaluator = evaluator;
                                process.PerformanceRecord = performance;
                                return process;
                            },
                            "EvaluatorId, PerformanceRecordId, CreatorId");
                    }

                    return performanceAppraisal;
                }
            }
        }

        public async Task<IEnumerable<PARecordModel>> GetPARecordByReviewerAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = await db.QueryMultipleAsync(criteria).ConfigureAwait(false))
                {
                    var performanceAppraisals = multi.Read<PARecordModel, Employee, PARecordModel>(
                        (pa, employee) =>
                        {
                            pa.Employee = employee;
                            return pa;
                        },
                        "EmployeeId");

                    var processes = multi.Read<PAProcessModel, Employee, PerformanceRecord, PAProcessModel>(
                            (process, evaluator, performance) =>
                            {
                                process.Evaluator = evaluator;
                                process.PerformanceRecord = performance;
                                return process;
                            },
                            "EvaluatorId, PerformanceRecordId");

                    MappingData(performanceAppraisals, processes);

                    return performanceAppraisals;
                }
            }
        }

        private static void MappingData(IEnumerable<PARecordModel> performanceAppraisals, IEnumerable<PAProcessModel> processes)
        {
            foreach (PARecordModel performanceAppraisal in performanceAppraisals)
            {
                performanceAppraisal.Processes = processes.Where(x => x.PARecordId == performanceAppraisal.PARecordId);
            }
        }
    }
}