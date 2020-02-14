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
                            "EmployeeId, EmployeeId");
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

                        MappingData(performanceAppraisal, performanceRecords, projects, reactions, feedbacks, feedbackReactions);
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

                    MappingData(performanceAppraisals, processes);

                    return performanceAppraisals;
                }
            }
        }

        private static void MappingData(
            PARecordModel performanceAppraisal,
            IEnumerable<PerformanceRecord> performanceRecords,
            IEnumerable<ProjectModel> projects,
            IEnumerable<PRReactionModel> reactions,
            IEnumerable<PRFeedbackModel> feedbacks,
            IEnumerable<PRFeedbackReactionModel> feedbackReactions)
        {
            foreach (PRFeedbackModel feedback in feedbacks)
            {
                feedback.Reactions = feedbackReactions.Where(x => x.PRFeedbackId == feedback.PRFeedbackId);
            }

            foreach (PerformanceRecord performanceRecord in performanceRecords)
            {
                performanceRecord.Projects = projects.Where(x => x.PerformanceRecordId == performanceRecord.PerformanceRecordId);
                performanceRecord.Reactions = reactions.Where(x => x.PerformanceRecordId == performanceRecord.PerformanceRecordId);
                performanceRecord.Feedbacks = feedbacks.Where(x => x.PerformanceRecordId == performanceRecord.PerformanceRecordId);
            }

            foreach (PAProcessModel process in performanceAppraisal.Processes)
            {
                var recordIds = !string.IsNullOrWhiteSpace(process.RelatedPerformanceRecordIds)
                    ? process.RelatedPerformanceRecordIds.Split(',') : new string[] { };
                process.RelatedPerformanceRecords = performanceRecords.Where(x => recordIds.Any(y => y == x.PerformanceRecordId.ToString()));
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