namespace NexGoal.Tools
{
    using System.Collections.Generic;

    public class EPAProcessModel
    {
        public EPAProcessModel()
        {
            EPAGroupDetails = new List<EPAGroupDetailModel>();
        }

        public int EPAId { get; set; }

        public int ProcessId { get; set; }

        public int EvaluatorId { get; set; }

        public string TotalScore { get; set; }

        public string TotalScoreEncode { get; set; }

        public string AdjustScore { get; set; }

        public string AdjustScoreEncode { get; set; }

        public int StatusId { get; set; }

        public IList<EPAGroupDetailModel> EPAGroupDetails { get; set; }
    }
}