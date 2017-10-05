namespace NexGoal.Model.Period
{
    using NexGoal.Core.Services;

    public class AutoEmployeePeriodModel
    {
        public int EPAId { get; set; }

        public string EmployeeIds { get; set; }

        public int PeriodId { get; set; }

        public int TargetTypeId { get; set; }

        public int ProcessId { get; set; }

        public string TotalScoreEncode { get; set; }

        public string TotalScore
        {
            get
            {
                ICertificateService certificateService = new CertificateService();

                return certificateService.Decrypt(TotalScoreEncode);
            }
        }

        public int EmployeeId { get; set; }
    }
}