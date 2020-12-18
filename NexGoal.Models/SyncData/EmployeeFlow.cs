namespace NexGoal.Models.SyncData
{
    using System;

    [Serializable]
    public class EmployeeFlow
    {
        public int EmployeeId { get; set; }
        public byte ProcessId { get; set; }
        public int EvaluatorId { get; set; }
    }
}