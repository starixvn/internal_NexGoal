namespace NexGoal.Tools
{
    public class EPADetailModel
    {
        public int EPAId { get; set; }

        public int ProcessId { get; set; }

        public int EvaluatorId { get; set; }

        public int KPIGroupId { get; set; }

        public int KPIId { get; set; }

        public string Score { get; set; }

        public string ScoreEncode { get; set; }

        public string Rate { get; set; }

        public string RateEncode { get; set; }
    }
}