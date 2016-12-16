using NexGoal.Model.EmployeeEPA;
using NexGoal.Model.Integration;
using System.Collections.Generic;

namespace NexGoal.Model.Export
{
    public class EpaDetailModel
    {
        public EPAModel Epa { get; set; }

        public IEnumerable<HRMSummaryViewModel> Hrm { get; set; }

        public IEnumerable<StandardGradeModel> Grades { get; set; }

        public string RootApplication { get; set; }
    }
}
