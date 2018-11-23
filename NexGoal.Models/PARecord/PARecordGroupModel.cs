namespace NexGoal.Models.PARecord
{
    using System.Collections.Generic;

    public class PARecordGroupModel
    {
        public string GroupName { get; set; }

        public IEnumerable<PARecordModel> PerformanceAppraisals { get; set; }
    }
}