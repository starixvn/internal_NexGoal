namespace NexGoal.Model.EmployeeEPA
{
    using NexGoal.Core.Constants;
    using System.Collections.Generic;

    public class EPAKPIGroupModel
    {
        public EPAKPIGroupModel()
        {
            KPIs = new List<EPAKPIModel>();
            EvaluatorsScore = new List<EvaluationModel>();
        }

        public int EPAId { get; set; }

        public int KPIGroupId { get; set; }

        public string Name { get; set; }

        public double? Weight { get; set; }

        public bool IsSpreadKPI { get; set; }

        public int DisplayOrder { get; set; }

        public int NumberKPI { get; set; }

        public bool HasKPIOptional { get; set; }

        public int NumberKPIOptional { get; set; }

        public IList<EPAKPIModel> KPIs { get; set; }

        public IList<EvaluationModel> EvaluatorsScore { get; set; }

        public void Decode()
        {
            foreach (var evaluator in EvaluatorsScore)
            {
                evaluator.Decode();
            }
        }
    }
}