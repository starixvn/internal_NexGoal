namespace NexGoal.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using NexGoal.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.PARecord;
    using NexGoal.Models.PerformanceRecord;
    using NexGoal.Models.PRFeedback;
    using NexGoal.Models.PRFeedbackReaction;
    using NexGoal.Models.PRReaction;

#pragma warning disable S1192 // String literals should not be duplicated
    public class PARepository : IPARepository
    {
        public async Task<IEnumerable<PerformanceAppraisalResult>> GetPerformanceAppraisalResultAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return await db.QueryAsync<PerformanceAppraisalResult, PeriodBaseModel, PerformanceAppraisalResult>(
                    (pa, period) =>
                    {
                        pa.Period = period;
                        return pa;
                    },
                    "PeriodId",
                    criteria);
            }
        }

        public async Task<IEnumerable<PerformanceAppraisalResult>> GetMemberPerformanceAppraisalResultAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return await db.QueryAsync<PerformanceAppraisalResult, Employee, PeriodBaseModel, PerformanceAppraisalResult>(
                    (pa, employee, period) =>
                    {
                        pa.Employee = employee;
                        pa.Period = period;
                        return pa;
                    },
                    "EmployeeId, PeriodId",
                    criteria);
            }
        }

        public async Task<PARecordModel> GetPARecordByIdAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = await db.QueryMultipleAsync(criteria))
                {
                    var performanceAppraisal = multi.Read<PARecordModel, Employee, PeriodBaseModel, PARecordModel>(
                        (pa, employee, period) =>
                        {
                            pa.Employee = employee;
                            pa.Period = period;
                            return pa;
                        },
                        "EmployeeId, PeriodId").FirstOrDefault();

                    if (performanceAppraisal != null)
                    {
                        performanceAppraisal.Processes = multi.Read<PAProcessModel, Employee, PAProcessModel>(
                            (process, evaluator) =>
                            {
                                process.Evaluator = evaluator;
                                return process;
                            },
                            "EvaluatorId");
                        var performanceRecords = multi.Read<PerformanceRecord, Employee, PerformanceRecord>(
                            (record, creator) =>
                            {
                                record.Creator = creator;
                                return record;
                            },
                            "EmployeeId");
                        var projects = multi.Read<ProjectModel>();
                        var reactions = multi.Read<PRReactionModel, Employee, PRReactionModel>(
                            (reaction, creator) =>
                            {
                                reaction.Creator = creator;
                                return reaction;
                            },
                            "EmployeeId");
                        var feedbacks = multi.Read<PRFeedbackModel, Employee, PRFeedbackModel>(
                            (feedback, creator) =>
                            {
                                feedback.Creator = creator;
                                return feedback;
                            },
                            "EmployeeId");
                        var feedbackReactions = multi.Read<PRFeedbackReactionModel, Employee, PRFeedbackReactionModel>(
                            (reaction, creator) =>
                            {
                                reaction.Creator = creator;
                                return reaction;
                            },
                            "EmployeeId");

                        CommonMapping.MappingPerformanceRecord(performanceRecords, projects, reactions, feedbacks, feedbackReactions);
                        CommonMapping.MappingRelatedPerformanceRecord(performanceAppraisal.Processes, performanceRecords);
                    }

                    return performanceAppraisal;
                }
            }
        }

        public async Task<IEnumerable<PARecordModel>> GetPARecordByReviewerAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = await db.QueryMultipleAsync(criteria))
                {
                    var performanceAppraisals = multi.Read<PARecordModel, Employee, PARecordModel>(
                        (pa, employee) =>
                        {
                            pa.Employee = employee;
                            return pa;
                        },
                        "EmployeeId");
                    var processes = multi.Read<PAProcessModel, Employee, PAProcessModel>(
                        (process, evaluator) =>
                        {
                            process.Evaluator = evaluator;
                            return process;
                        },
                        "EvaluatorId");
                    CommonMapping.MappingPerformanceAppraisal(performanceAppraisals, processes);

                    return performanceAppraisals;
                }
            }
        }
    }
}