namespace NexGoal.Model.Report.EPASummary
{
    using System.Collections.Generic;
    using NexGoal.Core.Constants;
    using NexGoal.Model.Period;

    public class EPASummaryModel
    {
        public EPASummaryModel()
        {
            PeriodSelecteds = new List<PeriodModel>();
            EPAInfos = new List<EPASummaryEmployeeModel>();
        }

        public IList<PeriodModel> PeriodSelecteds { get; set; }

        public IList<EPASummaryEmployeeModel> EPAInfos { get; set; }

        public static int MaxFinalScore => Constants.MaxFinalScore;

        public int TotalItem { get; set; }
    }
}