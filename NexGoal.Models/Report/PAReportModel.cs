namespace NexGoal.Models.Report
{
    using System.Collections.Generic;
    using NexGoal.Models.Common;
    using NexGoal.Models.PARecord;

    public class PAReportModel
    {
        public GroupBaseModel Group { get; set; }

        public IEnumerable<PARecordModel> PARecords { get; set; }
    }
}