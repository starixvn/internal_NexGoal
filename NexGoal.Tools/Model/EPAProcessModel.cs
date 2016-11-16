namespace NexGoal.Tools
{
    using System.Collections.Generic;

    public class EPAProcessModel
    {
        public int EPAId { get; set; }

        public int ProcessId { get; set; }

        public int EvaluatorId { get; set; }

        public string TotalScore { get; set; }

        public string AdjustScore { get; set; }

        public int StatusId { get; set; }        
    }
}