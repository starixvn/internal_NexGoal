using NexGoal.Model.EmployeeEPA;
using NexGoal.Model.Integration;
using NexGoal.Model.Violation;
using System.Collections.Generic;

namespace NexGoal.Model.Export
{
    public class EpaDetailModel
    {
        public EPAModel Epa { get; set; }

        public IEnumerable<HRMSummaryViewModel> Hrm { get; set; }

        public IEnumerable<StandardGradeModel> Grades { get; set; }

        public IEnumerable<EmployeeWorkingViewModel> NexPMs { get; set; }

        public IEnumerable<ViolationViewModel> Violations { get; set; }

        public IEnumerable<NominationEPAModel> Nominations { get; set; }

        public IEnumerable<EPAKPIGroupModel> LastRatings { get; set; }
    }
}
