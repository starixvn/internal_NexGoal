namespace NexGoal.Models.Report
{
    using System.Collections.Generic;
    using NexGoal.Models.Common;
    using NexGoal.Models.PARecord;

    public class PAReportModel
    {
        public Group Group { get; set; }

        public IEnumerable<PARecordModel> PARecords { get; set; }
    }
}