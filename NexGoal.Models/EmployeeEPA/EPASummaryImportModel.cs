namespace NexGoal.Model.EmployeeEPA
{
    using NexGoal.Core.Services;

    public class EPASummaryImportModel
    {
        public int Id { get; set; }

        public float OriginFinalScore { get; set; }

        public string FinalScore
        {
            get
            {
                ICertificateService certificateService = new CertificateService();
                string cipherText = certificateService.Encrypt(OriginFinalScore.ToString());

                return cipherText;
            }
        }
    }
}