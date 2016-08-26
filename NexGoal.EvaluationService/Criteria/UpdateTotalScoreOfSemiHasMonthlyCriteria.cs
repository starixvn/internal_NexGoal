namespace NexGoal.AutoGenerateEvaluation
{
    using System.Text;
    using Fanex.Data.Repository;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Services;
    using NexGoal.Model.EmployeeEPA;

    public class UpdateTotalScoreOfSemiHasMonthlyCriteria : CriteriaBase
    {
        private const string EPAXmlTemplate = "<EPA ProcessId='{0}' TotalScore='{1}'></EPA>";
        private const string BeginRootTag = "<ROOT>";
        private const string EndRootTag = "</ROOT>";

        public int EPAId { get; set; }

        [SpParam(Ignored = true)]
        public MonthlyEPAModel MonthlyEPA { get; set; }

        public string Details
        {
            get
            {
                var epaXml = new StringBuilder(BeginRootTag);
                ICertificateService certificateService = new CertificateService();

                epaXml.AppendFormat(
                        EPAXmlTemplate,
                        (int)ProcessType.Self,
                        certificateService.Encrypt(MonthlyEPA.AverageSelfScoreConverted.ToString()));

                epaXml.AppendFormat(
                        EPAXmlTemplate,
                        (int)ProcessType.Evaluator,
                        certificateService.Encrypt(MonthlyEPA.AverageEvaluatorScoreConverted.ToString()));

                epaXml.AppendFormat(
                        EPAXmlTemplate,
                        (int)ProcessType.Reviewer,
                        certificateService.Encrypt(MonthlyEPA.AverageReviewerScoreConverted.ToString()));

                epaXml.Append(EndRootTag);

                return epaXml.ToString();
            }
        }

        public override bool IsValid()
        {
            return EPAId > 0;
        }

        public override string GetSettingKey()
        {
            return "EpaAuto_SemiAnnual_UpdateTotalScore";
        }
    }
}