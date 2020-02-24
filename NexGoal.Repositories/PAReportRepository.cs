namespace NexGoal.Repositories
{
    using System;
    using System.Collections.Generic;
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
                    CommonMapping.MappingPerformanceAppraisal(performanceAppraisals, processes);

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
                    CommonMapping.MappingPerformanceRecord(performanceRecords, projects, reactions, feedbacks, feedbackReactions);
                    CommonMapping.MappingRelatedPerformanceRecord(processes, performanceRecords);
                    CommonMapping.MappingPerformanceAppraisal(performanceAppraisals, processes);

                    return new Tuple<IEnumerable<PeriodBaseModel>, IEnumerable<PARecordModel>>(selectedPeriods, performanceAppraisals);
                }
            }
        }
    }
}