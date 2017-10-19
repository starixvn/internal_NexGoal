namespace NexGoal.Model.EmployeeEPA
{
    using System.Globalization;
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
                string cipherText = certificateService.Encrypt(OriginFinalScore.ToString(CultureInfo.InvariantCulture));

                return cipherText;
            }
        }
    }
}