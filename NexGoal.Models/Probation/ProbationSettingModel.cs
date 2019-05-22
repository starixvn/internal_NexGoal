namespace NexGoal.Models.Probation
{
    using NexGoal.Core.Extensions;
    using System;

    [XMLName("Row")]
    public class ProbationSettingModel
    {
        public int EmployeeId { get; set; }

        public int ProcessId { get; set; }

        public int EvaluatorId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
    }
}