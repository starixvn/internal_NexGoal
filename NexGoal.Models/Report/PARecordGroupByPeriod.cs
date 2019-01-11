namespace NexGoal.Models.Report
{
    using System.Collections.Generic;
    using NexGoal.Models.Common;

    public class PARecordGroupByPeriod
    {
        public int PeriodId { get; set; }

        public PeriodBaseModel Period { get; set; }

        public IEnumerable<PAReportModel> Groups { get; set; }
    }
}