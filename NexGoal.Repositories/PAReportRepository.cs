namespace NexGoal.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using NexGoal.Models.Common;
    using NexGoal.Models.PARecord;

    public class PAReportRepository : IPAReportRepository
    {
        public async Task<Tuple<IEnumerable<PARecordModel>, IEnumerable<PeriodBaseModel>>> GetPAReportAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = await db.QueryMultipleAsync(criteria))
                {
                    var performanceAppraisals = multi.Read<PARecordModel, Employee, PeriodBaseModel, PARecordModel>(
                        (pa, employee, period) =>
                        {
                            pa.Employee = employee;
                            pa.Period = period;
                            return pa;
                        },
                        "EmployeeId, PeriodId");

                    var processes = multi.Read<PAProcessModel, Employee, PAProcessModel>(
                            (process, evaluator) =>
                            {
                                process.Evaluator = evaluator;
                                return process;
                            },
                            "EvaluatorId");

                    var selectedPeriods = multi.Read<PeriodBaseModel>();
                    MappingData(performanceAppraisals, processes);

                    return new Tuple<IEnumerable<PARecordModel>, IEnumerable<PeriodBaseModel>>(performanceAppraisals, selectedPeriods);
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