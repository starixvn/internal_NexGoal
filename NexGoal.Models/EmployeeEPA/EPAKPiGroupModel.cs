namespace NexGoal.Model.EmployeeEPA
{
    using NexGoal.Core.Services;
    using NexGoal.Model.Enitites;
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

        public string SelfEncodeScore { get; set; }

        public float SelfDecodeScore { get; set; }

        public void Decode()
        {
            SelfDecodeScore = string.IsNullOrEmpty(SelfEncodeScore) ? 0 : float.Parse(EncodeService.Decode(SelfEncodeScore));
            foreach (var evaluator in EvaluatorsScore)
            {
                evaluator.Decode();
            }

            foreach (var kpi in KPIs)
            {
                kpi.Decode();
            }
        }
    }
}