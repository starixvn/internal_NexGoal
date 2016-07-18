namespace NexGoal.Model.Report.EPADetail
{
    using System.Collections.Generic;
    using NexGoal.Model.EmployeeEPA;

    public class EPADetailModel
    {
        public IEnumerable<EPAModel> MemberEPAs { get; set; }

        public IEnumerable<EvaluationPhaseModel> PhaseSelecteds { get; set; }

        public int TotalItem { get; set; }
    }
}