namespace NexGoal.Models.Report
{
    using System.Collections.Generic;
    using NexGoal.Models.Common;
    using NexGoal.Models.PARecord;

    public class PARecordGroupByPeriod
    {
        public int PeriodId { get; set; }

        public PeriodBaseModel Period { get; set; }

        public IEnumerable<PARecordModel> PARecords { get; set; }
    }
}