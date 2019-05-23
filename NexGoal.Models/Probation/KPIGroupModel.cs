namespace NexGoal.Models.Probation
{
    using System.Collections.Generic;

    public class KPIGroupModel
    {
        public int KPIGroupId { get; set; }

        public string Name { get; set; }

        public IEnumerable<ProbationEvaluationDetailModel> Details { get; set; }
    }
}