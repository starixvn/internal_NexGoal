namespace NexGoal.Models.EmployeeNomination
{
    using System.Collections.Generic;
    using NexGoal.Models.Common;
    using NexGoal.Models.PerformanceRecord;

    public class NominationModel
    {
        public int NominationId { get; set; }

        public int StatusId { get; set; }

        public int NomineeId { get; set; }

        public EmployeeBaseModel Nominee { get; set; }

        public int NominationPeriodId { get; set; }

        public NominationPeriodModel Period { get; set; }

        public IEnumerable<PerformanceRecordModel> Nominators { get; set; }
    }
}