namespace NexGoal.Model.Report.EPADetail
{
    using NexGoal.Core.Services;
    using NexGoal.Model.EmployeeEPA;
    using System.Collections.Generic;

    public class EPADetailModel
    {
        public EPADetailModel()
        {
            this.Process = new EvaluationProcessModel();
        }

        public int EPAId { get; set; }

        public EPAEmployeeModel Employee { get; set; }

        public EPAPeriodModel Period { get; set; }

        public EPAStatusModel Status { get; set; }

        public string FinalScoreEncode { get; set; }

        public string FinalScore { get; set; }

        public string GradeEncode { get; set; }

        public string Grade { get; set; }

        public EvaluationProcessModel Process { get; set; }

        public int TotalItem { get; set; }

        public void Decrypt()
        {
            ICertificateService certificateService = new CertificateService();

            FinalScore = certificateService.Decrypt(FinalScoreEncode);
            Grade = certificateService.Decrypt(GradeEncode);
            Process.Self.Decrypt();
            DecodeForList(Process.Evaluators);
            DecodeForList(Process.Reviewers);
        }

        private void DecodeForList(IList<EPAProcessModel> processes)
        {
            foreach (EPAProcessModel process in processes)
            {
                process.Decrypt();
            }
        }
    }
}