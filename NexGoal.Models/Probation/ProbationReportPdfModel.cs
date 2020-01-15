namespace NexGoal.Models.Probation
{
    using System.Collections.Generic;

    public class ProbationReportPdfModel : ProbationReportModel
    {
        public IEnumerable<KPIGroupModel> Details { get; set; }
        public HRMModel Hrm { get; set; }
    }
}