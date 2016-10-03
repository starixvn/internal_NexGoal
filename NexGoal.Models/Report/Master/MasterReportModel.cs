using NexGoal.Model.Integration;
using NexGoal.Model.PAFocus;
using System.Collections.Generic;

namespace NexGoal.Model.Report
{
    public class MasterReportModel
    {
        public MasterReportModel()
        {
            Nominations = new List<NominationReportModel>();

            EmployeeWorkings = new List<EmployeeWorkingViewModel>();
        }

        public string ReviewerAdjustment { get; set; }

        public string EvaluatorComment { get; set; }

        public PAFocusModel PAFocus { get; set; }

        public List<NominationReportModel> Nominations { get; set; }

        public List<EmployeeWorkingViewModel> EmployeeWorkings { get; set; }

        public void Decrypt()
        {
            PAFocus.Decrypt();
        }
    }
}
