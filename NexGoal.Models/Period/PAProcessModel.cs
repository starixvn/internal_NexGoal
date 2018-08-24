namespace NexGoal.Models.Period
{
    using System;

    public class PAProcessModel
    {
        public int PARecordId { get; set; }

        public int ProcessId { get; set; }

        public string Process { get; set; }

        public int EvaluatorId { get; set; }

        public string Evaluator { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int? StatusId { get; set; }

        public string Status { get; set; }
    }
}