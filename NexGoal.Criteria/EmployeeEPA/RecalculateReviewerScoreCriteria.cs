namespace NexGoal.Criteria.EmployeeEPA
{
    using Core.Constants;
    using Core.Services;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Model.EmployeeEPA;
    using System.Collections.Generic;
    using System.Text;
    using System.Web;

    public class RecalculateReviewerScoreCriteria : CriteriaBase
    {
        private const string XmlTemplate = @"<EPA EPAId='{0}' ProcessId='{1}' EmployeeId='{2}'
            EvaluatorId='{3}' AdjustScore='{4}' TotalScore='{5}' Comment='{6}'></EPA>";

        public string EPADetails
        {
            get
            {
                var epaXml = new StringBuilder(Constants.BeginRootTag);
                ICertificateService certificateService = new CertificateService();

                foreach (EPAModel epaInfo in EPAInfos)
                {
                    foreach (EPAProcessModel process in epaInfo.Process.Reviewers)
                    {
                        var cipherTotalScore = certificateService.Encrypt(process.TotalScore);
                        var cipherAdjustScore = certificateService.Encrypt(process.AdjustScore);

                        epaXml.AppendFormat(
                        XmlTemplate,
                        epaInfo.EPAId,
                        process.ProcessId,
                        epaInfo.Employee.EmployeeId,
                        process.EvaluatorId,
                        cipherAdjustScore,
                        cipherTotalScore,
                        HttpUtility.HtmlEncode(process.Comment));
                    }
                }

                epaXml.Append(Constants.EndRootTag);

                return epaXml.ToString();
            }
        }

        public int ModifiedBy { get; set; }

        [SpParam(Ignored = true)]
        public IEnumerable<EPAModel> EPAInfos { get; set; }

        public override bool IsValid()
        {
            return ModifiedBy > 0 && !string.IsNullOrWhiteSpace(EPADetails);
        }

        public override string GetSettingKey()
        {
            return "EPA_AdjustScoreByReviewerOrManagement";
        }
    }
}