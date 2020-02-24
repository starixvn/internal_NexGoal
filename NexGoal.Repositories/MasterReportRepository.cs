namespace NexGoal.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using NexGoal.Models.Common;
    using NexGoal.Models.MasterReport;
    using NexGoal.Models.PARecord;
    using NexGoal.Models.PerformanceRecord;
    using NexGoal.Models.PRFeedback;
    using NexGoal.Models.PRFeedbackReaction;
    using NexGoal.Models.PRReaction;

    public interface IMasterReportRepository
    {
        Task<IEnumerable<MasterReportModel>> GetMasterReportAsync(ICriteria criteria);
    }

#pragma warning disable S1192 // String literals should not be duplicated
    public class MasterReportRepository : IMasterReportRepository
    {
        public async Task<IEnumerable<MasterReportModel>> GetMasterReportAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = await db.QueryMultipleAsync(criteria))
                {
                    var records = multi.Read<MasterReportModel>();
                    var awards = multi.Read<AwardModel>();
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

                    CommonMapping.MappingPerformanceRecord(performanceRecords, reactions: reactions, feedbacks: feedbacks, feedbackReactions: feedbackReactions);
                    CommonMapping.MappingRelatedPerformanceRecord(processes, performanceRecords);
                    foreach (MasterReportModel record in records)
                    {
                        record.Awards = awards.Where(x => x.NomineeId == record.EmployeeId);
                        record.Processes = processes.Where(x => x.PARecordId == record.PARecordId).OrderBy(x => x.ProcessId);
                    }

                    return records;
                }
            }
        }
    }
}