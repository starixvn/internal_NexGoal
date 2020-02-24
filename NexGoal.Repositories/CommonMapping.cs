namespace NexGoal.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using NexGoal.Models;
    using NexGoal.Models.PARecord;
    using NexGoal.Models.PerformanceRecord;
    using NexGoal.Models.PRFeedback;
    using NexGoal.Models.PRFeedbackReaction;
    using NexGoal.Models.PRReaction;

    public static class CommonMapping
    {
        public static void MappingPerformanceRecord(
            IEnumerable<PerformanceRecord> performanceRecords,
            IEnumerable<ProjectModel> projects = null,
            IEnumerable<PRReactionModel> reactions = null,
            IEnumerable<PRFeedbackModel> feedbacks = null,
            IEnumerable<PRFeedbackReactionModel> feedbackReactions = null)
        {
            foreach (PRFeedbackModel feedback in feedbacks)
            {
                feedback.Reactions = feedbackReactions?.Where(x => x.PRFeedbackId == feedback.PRFeedbackId);
            }

            foreach (PerformanceRecord performanceRecord in performanceRecords)
            {
                performanceRecord.Projects = projects?.Where(x => x.PerformanceRecordId == performanceRecord.PerformanceRecordId);
                performanceRecord.Reactions = reactions?.Where(x => x.PerformanceRecordId == performanceRecord.PerformanceRecordId);
                performanceRecord.Feedbacks = feedbacks?.Where(x => x.PerformanceRecordId == performanceRecord.PerformanceRecordId);
            }
        }

        public static void MappingRelatedPerformanceRecord(
            IEnumerable<PAProcessModel> processes,
            IEnumerable<PerformanceRecord> performanceRecords)
        {
            foreach (PAProcessModel process in processes)
            {
                var recordIds = !string.IsNullOrWhiteSpace(process.RelatedPerformanceRecordIds)
                    ? process.RelatedPerformanceRecordIds.Split(',') : new string[] { };
                process.RelatedPerformanceRecords = performanceRecords.Where(x => recordIds.Any(y => y == x.PerformanceRecordId.ToString()))
                    .OrderByDescending(x => x.CreatedDate);
            }
        }

        public static void MappingPerformanceAppraisal(
            IEnumerable<PARecordModel> performanceAppraisals,
            IEnumerable<PAProcessModel> processes)
        {
            foreach (PARecordModel performanceAppraisal in performanceAppraisals)
            {
                performanceAppraisal.Processes = processes.Where(x => x.PARecordId == performanceAppraisal.PARecordId).OrderBy(x => x.ProcessId);
            }
        }
    }
}