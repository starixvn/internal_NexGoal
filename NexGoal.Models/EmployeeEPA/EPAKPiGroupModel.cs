namespace NexGoal.Model.EmployeeEPA
{
    using NexGoal.Core.Services;
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

        public void Decrypt()
        {
            ICertificateService certificateService = new CertificateService();
            SelfDecodeScore = string.IsNullOrEmpty(SelfEncodeScore) ? 0 : float.Parse(certificateService.Decrypt(SelfEncodeScore));
            foreach (var evaluator in EvaluatorsScore)
            {
                evaluator.Decrypt();
            }

            foreach (var kpi in KPIs)
            {
                kpi.Decrypt();
            }
        }
    }
}