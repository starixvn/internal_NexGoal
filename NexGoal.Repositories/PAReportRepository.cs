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

    public class PAReportRepository : IPAReportRepository
    {
        public async Task<IEnumerable<PARecordModel>> GetPARecordByReviewerAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = await db.QueryMultipleAsync(criteria).ConfigureAwait(false))
                {
                    var performanceAppraisals = multi.Read<PARecordModel, EmployeeBaseModel, PeriodBaseModel, PARecordModel>(
                        (pa, employee, period) =>
                        {
                            pa.Employee = employee;
                            pa.Period = period;
                            return pa;
                        },
                        "EmployeeId, PeriodId");

                    var processes = multi.Read<PAProcessModel, EmployeeBaseModel, PerformanceRecordModel, PAProcessModel>(
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