namespace NexGoal.Model.Dashboard
{
    public class EPAInfoModel
    {
        public int PeriodId { get; set; }

        public string PeriodName { get; set; }

        public int UnfinishedByEvaluator { get; set; }

        public int FinishedByEvaluator { get; set; }

        public int UnfinishedByReviewer { get; set; }

        public int FinishedByReviewer { get; set; }
    }
}