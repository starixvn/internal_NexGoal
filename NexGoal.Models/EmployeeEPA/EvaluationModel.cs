using NexGoal.Core.Services;

namespace NexGoal.Model.EmployeeEPA
{
    public class EvaluationModel
    {
        public int EPAId { get; set; }

        public int ProcessId { get; set; }

        public int KPIId { get; set; }

        public int KPIGroupId { get; set; }

        public int EvaluatorId { get; set; }

        public string Code { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public string ScoreEncode { get; set; }

        public double Score { get; set; }

        public string RateEncode { get; set; }

        public float Rate { get; set; }

        public string Comment { get; set; }

        public bool IsMaxScore { get; set; }

        public double Weight { get; set; }

        public void Decrypt()
        {
            ICertificateService certificateService = new CertificateService();

            Score = string.IsNullOrEmpty(ScoreEncode) ? 0 : double.Parse(certificateService.Decrypt(ScoreEncode));
            Rate = string.IsNullOrEmpty(RateEncode) ? 0 : float.Parse(certificateService.Decrypt(RateEncode));
        }
    }
}