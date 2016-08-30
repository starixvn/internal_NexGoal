namespace NexGoal.Tools
{
    using System.Collections.Generic;

    public class EPAGroupDetailModel
    {
        public EPAGroupDetailModel()
        {
            EPADetails = new List<EPADetailModel>();
        }

        public int EPAId { get; set; }

        public int ProcessId { get; set; }

        public int EvaluatorId { get; set; }

        public int KPIGroupId { get; set; }

        public string Score { get; set; }

        public string ScoreEncode { get; set; }

        public IList<EPADetailModel> EPADetails { get; set; }
    }
}