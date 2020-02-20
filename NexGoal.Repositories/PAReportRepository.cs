namespace NexGoal.Repositories
{
    using System;
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

    public interface IPAReportRepository
    {
        Task<Tuple<IEnumerable<PeriodBaseModel>, IEnumerable<PARecordModel>>> GetPAReportAsync(ICriteria criteria);

        Task<Tuple<IEnumerable<PeriodBaseModel>, IEnumerable<PARecordModel>>> GetDataForExportPAReportAsync(ICriteria criteria);
    }

#pragma warning disable S1192 // String literals should not be duplicated
    public class PAReportRepository : IPAReportRepository
    {
        public async Task<Tuple<IEnumerable<PeriodBaseModel>, IEnumerable<PARecordModel>>> GetPAReportAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = await db.QueryMultipleAsync(criteria))
                {
                    var selectedPeriods = multi.Read<PeriodBaseModel>();
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

                    return new Tuple<IEnumerable<PeriodBaseModel>, IEnumerable<PARecordModel>>(selectedPeriods, performanceAppraisals);
                }
            }
        }

        public async Task<Tuple<IEnumerable<PeriodBaseModel>, IEnumerable<PARecordModel>>> GetDataForExportPAReportAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = await db.QueryMultipleAsync(criteria))
                {
                    var selectedPeriods = multi.Read<PeriodBaseModel>();
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
                    MappingData(performanceAppraisals, processes, performanceRecords, projects, reactions, feedbacks, feedbackReactions);

                    return new Tuple<IEnumerable<PeriodBaseModel>, IEnumerable<PARecordModel>>(selectedPeriods, performanceAppraisals);
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

        private static void MappingData(
            IEnumerable<PARecordModel> performanceAppraisals,
            IEnumerable<PAProcessModel> processes,
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

            foreach (PAProcessModel process in processes)
            {
                var recordIds = !string.IsNullOrWhiteSpace(process.RelatedPerformanceRecordIds)
                    ? process.RelatedPerformanceRecordIds.Split(',') : new string[] { };
                process.RelatedPerformanceRecords = performanceRecords.Where(x => recordIds.Any(y => y == x.PerformanceRecordId.ToString()))
                    .OrderByDescending(x => x.CreatedDate);
            }

            foreach (PARecordModel performanceAppraisal in performanceAppraisals)
            {
                performanceAppraisal.Processes = processes.Where(x => x.PARecordId == performanceAppraisal.PARecordId).OrderBy(x => x.ProcessId);
            }
        }
    }
}