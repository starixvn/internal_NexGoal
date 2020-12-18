namespace NexGoal.Models.SyncData
{
    using System;

    [Serializable]
    public class GroupFlow
    {
        public int GroupId { get; set; }
        public byte ProcessId { get; set; }
        public int EvaluatorId { get; set; }
    }
}