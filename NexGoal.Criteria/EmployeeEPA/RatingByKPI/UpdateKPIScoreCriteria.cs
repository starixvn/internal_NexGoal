namespace NexGoal.Criteria.EmployeeEPA.RatingByKPI
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using System.Web;
    using Core.Constants;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using NexGoal.Core.Services;
    using NexGoal.Model.EmployeeEPA.RatingByKPI;

    public class UpdateKPIScoreCriteria : CriteriaBase
    {
        private const string EPAXmlTemplate = "<EPA EPAId='{0}' EmployeeId='{1}' KPIGroupId='{2}' TotalScore='{3}'>{4}</EPA>";
        private const string EPADetailXmlTemplate = "<EPADetail KPIId ='{0}' Rate='{1}' Score='{2}' IsMaxScore='{3}' Comment='{4}' />";

        public int EvaluatorId { get; set; }

        public string KPIDetails
        {
            get
            {
                return ConvertToXml(EpaInfos);
            }
        }

        [SpParam(Ignored = true)]
        public IEnumerable<RatingByKPIModel> EpaInfos { get; set; }

        public override string GetSettingKey()
        {
            return "EPA_RatingByKPI_UpdateKPIScore";
        }

        public override bool IsValid()
        {
            return EvaluatorId > 0;
        }

        private static string Encrypt(string plainText)
        {
            ICertificateService certificateService = new CertificateService();
            string cipherText = certificateService.Encrypt(plainText);

            return cipherText;
        }

        private static string ConvertToXml(IEnumerable<RatingByKPIModel> epaInfos)
        {
            var epaXml = new StringBuilder(Constants.BeginRootTag);

            foreach (RatingByKPIModel epaInfo in epaInfos)
            {
                var epaDetailXml = new StringBuilder();

                foreach (TemporaryKPIModel kpi in epaInfo.KPIGroup.KPIs)
                {
                    if (!string.IsNullOrEmpty(kpi.AdjustScore) || kpi.AdjustRate > 0)
                    {
                        epaDetailXml = GenerateEpaDetailXml(kpi, kpi.AdjustScore, kpi.AdjustRate);
                    }
                    else if (!string.IsNullOrEmpty(kpi.Score) || kpi.Rate > 0)
                    {
                        epaDetailXml = GenerateEpaDetailXml(kpi, kpi.Score, kpi.Rate);
                    }

                    string totalScore = Encrypt(epaInfo.TotalScore);
                    epaXml.AppendFormat(EPAXmlTemplate, epaInfo.EPAId, epaInfo.Employee.EmployeeId, epaInfo.KPIGroup.KPIGroupId, totalScore, epaDetailXml);
                }
            }

            epaXml.Append(Constants.EndRootTag);

            return epaXml.ToString();
        }

        private static StringBuilder GenerateEpaDetailXml(TemporaryKPIModel kpi, string score, float rate)
        {
            string kpiRate = rate > 0 ? rate.ToString(CultureInfo.InvariantCulture) : string.Empty;
            string cipherScore = Encrypt(score);
            string cipherRate = Encrypt(kpiRate);

            var epaDetailXml = new StringBuilder();
            epaDetailXml.AppendFormat(EPADetailXmlTemplate, kpi.KPIId, cipherRate, cipherScore, kpi.IsMaxScore, HttpUtility.HtmlEncode(kpi.Comment));

            return epaDetailXml;
        }
    }
}