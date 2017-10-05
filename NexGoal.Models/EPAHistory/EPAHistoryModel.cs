namespace NexGoal.Model.EPAHistory
{
    using NexGoal.Core.Services;
    using NexGoal.Model.EmployeeEPA;

    public class EPAHistoryModel
    {
        public EPAEmployeeModel Employee { get; set; }

        public EPAPeriodModel Period { get; set; }

        public double FinalScore { get; set; }

        public string EncodeFinalScore { get; set; }

        public string Grade { get; set; }

        public string EncodeGrade { get; set; }

        public bool HasEPAForm { get; set; }

        public int? EPAId { get; set; }

        public void Decrypt()
        {
            ICertificateService certificateService = new CertificateService();

            FinalScore = !string.IsNullOrWhiteSpace(EncodeFinalScore) ? double.Parse(certificateService.Decrypt(EncodeFinalScore)) : 0;
            Grade = !string.IsNullOrWhiteSpace(EncodeGrade) ? certificateService.Decrypt(EncodeGrade) : string.Empty;
        }
    }
}