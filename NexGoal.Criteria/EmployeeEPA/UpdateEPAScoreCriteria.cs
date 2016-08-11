namespace NexGoal.Criteria.EmployeeEPA
{
    using System.Collections.Generic;
    using System.Text;
    using System.Web;
    using Fanex.Data.Repository;
    using NexGoal.Core.Services;
    using NexGoal.Model.EmployeeEPA;

    public class UpdateEPAScoreCriteria : CriteriaBase
    {
        private const string KPIScoreXmlTemplate = "<KPI KPIId='{0}' KPIGroupId='{1}' Score='{2}' Rate='{3}' IsMaxScore='{4}' Comment='{5}'></KPI>";
        private const string BeginRootTag = "<ROOT>";
        private const string EndRootTag = "</ROOT>";

        public int EPAId { get; set; }

        public int ProcessId { get; set; }

        public int EmployeeId { get; set; }

        public int EvaluatorId { get; set; }

        public string Comment
        {
            get
            {
                return EPAProcess.Comment;
            }
        }

        public string Score
        {
            get
            {
                string totalScore = EPAProcess.TotalScore;
                return EncodeService.Encode(totalScore);
            }
        }

        public string KPIDetails
        {
            get
            {
                var kpiDetailXml = new StringBuilder(BeginRootTag);

                foreach (EPAKPIGroupModel kpiGroup in KPIGroups)
                {
                    foreach (EPAKPIModel kpi in kpiGroup.KPIs)
                    {
                        string score = kpi.CurrentEvaluation.Score > 0 ? kpi.CurrentEvaluation.Score.ToString() : string.Empty,
                            rate = kpi.CurrentEvaluation.Rate > 0 ? kpi.CurrentEvaluation.Rate.ToString() : string.Empty,
                            cipherScore = EncodeService.Encode(score),
                            cipherRate = EncodeService.Encode(rate);
                        bool isMaxScore = kpi.CurrentEvaluation.IsMaxScore;

                        kpiDetailXml.AppendFormat(
                            KPIScoreXmlTemplate,
                            kpi.KPIId,
                            kpiGroup.KPIGroupId,
                            cipherScore,
                            cipherRate,
                            isMaxScore,
                            HttpUtility.HtmlEncode(kpi.CurrentEvaluation.Comment));
                    }
                }

                kpiDetailXml.Append(EndRootTag);

                return kpiDetailXml.ToString();
            }
        }

        public int ModifiedBy { get; set; }

        [SpParam(Ignored = true)]
        public EPAProcessModel EPAProcess { get; set; }

        [SpParam(Ignored = true)]
        public IList<EPAKPIGroupModel> KPIGroups { get; set; }

        public override bool IsValid()
        {
            return EPAId > 0 && EmployeeId > 0 && EvaluatorId > 0 && ProcessId > 0;
        }

        public override string GetSettingKey()
        {
            return "EPA_UpdateScore";
        }
    }
}