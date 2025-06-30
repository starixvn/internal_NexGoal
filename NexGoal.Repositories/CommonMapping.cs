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
            foreach (var feedback in feedbacks)
            {
                feedback.Reactions = feedbackReactions?.Where(x => x.PRFeedbackId == feedback.PRFeedbackId);
            }

            foreach (var performanceRecord in performanceRecords)
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
            foreach (var process in processes)
            {
                var recordIds = !string.IsNullOrWhiteSpace(process.RelatedPerformanceRecordIds)
                    ? process.RelatedPerformanceRecordIds.Split(',').ToList() : new List<string>();
                process.RelatedPerformanceRecords = performanceRecords.Where(x => recordIds.Exists(y => y == x.PerformanceRecordId.ToString()))
                    .OrderByDescending(x => x.CreatedDate);
            }
        }

        public static void MappingPerformanceAppraisal(
            IEnumerable<PARecordModel> performanceAppraisals,
            IEnumerable<PAProcessModel> processes)
        {
            foreach (var performanceAppraisal in performanceAppraisals)
            {
                performanceAppraisal.Processes = processes.Where(x => x.PARecordId == performanceAppraisal.PARecordId).OrderBy(x => x.ProcessId);
            }
        }
    }
}