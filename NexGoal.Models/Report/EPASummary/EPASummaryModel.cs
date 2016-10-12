namespace NexGoal.Model.Report.EPASummary
{
    using NexGoal.Core.Constants;
    using NexGoal.Model.Period;
    using System.Collections.Generic;

    public class EPASummaryModel
    {
        public EPASummaryModel()
        {
            PeriodSelecteds = new List<PeriodModel>();
            EPAInfos = new List<EPASummaryEmployeeModel>();
        }

        public IList<PeriodModel> PeriodSelecteds { get; set; }

        public IList<EPASummaryEmployeeModel> EPAInfos { get; set; }

        public int MaxFinalScore
        {
            get
            {
                return Constants.MaxFinalScore;
            }
        }

        public int TotalItem { get; set; }
    }
}