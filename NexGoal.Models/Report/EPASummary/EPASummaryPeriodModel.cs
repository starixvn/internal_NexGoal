namespace NexGoal.Model.Report.EPASummary
{
    using NexGoal.Core.Services;
    using NexGoal.Model.EmployeeEPA;

    public class EPASummaryPeriodModel
    {
        public int PeriodId { get; set; }

        public int EmployeeId { get; set; }

        public string FinalScoreEncode { get; set; }

        public string FinalScore { get; set; }

        public bool HasDisciplineScore { get; set; }

        public float DisciplineScore { get; set; }

        public float MaxDisciplineScore { get; set; }

        public string GradeEncode { get; set; }

        public string Grade { get; set; }

        public bool IsEndPeriod { get; set; }

        public EPAStatusModel Status { get; set; }

        public void Decrypt()
        {
            ICertificateService certificateService = new CertificateService();

            FinalScore = certificateService.Decrypt(FinalScoreEncode);
            Grade = certificateService.Decrypt(GradeEncode);
        }
    }
}