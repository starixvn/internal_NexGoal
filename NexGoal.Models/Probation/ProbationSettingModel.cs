namespace NexGoal.Models.Probation
{
    using System;

    public class ProbationSettingModel
    {
        public int EmployeeId { get; set; }

        public byte ProcessId { get; set; }
        public string Process { get; set; }

        public int EvaluatorId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string EvaluatorNickName { get; set; }

        public string EvaluatorFullName { get; set; }
    }
}