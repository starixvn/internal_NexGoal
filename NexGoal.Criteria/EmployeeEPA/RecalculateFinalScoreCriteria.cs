namespace NexGoal.Criteria.EmployeeEPA
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Core.Constants;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Model.EmployeeEPA;

    public class RecalculateFinalScoreCriteria : CriteriaBase
    {
        private const string TemplateXml = "<EPA EPAId='{0}' FinalScore='{1}' DisciplineScore='{2}' Grade='{3}' ToeicPolicyScore='{4}'></EPA>";

        public int ModifiedBy { get; set; }

        [SpParam(Ignored = true)]
        public IEnumerable<EPAModel> EPAInfos { get; set; }

        public string EPAScores
        {
            get
            {
                var epaXmlBuilder = new StringBuilder();
                epaXmlBuilder.Append(Constants.BeginRootTag);

                if (EPAInfos != null && EPAInfos.Any())
                {
                    foreach (EPAModel epaInfo in EPAInfos)
                    {
                        epaXmlBuilder.AppendFormat(TemplateXml, epaInfo.EPAId, epaInfo.FinalScore, 0, epaInfo.Grade, epaInfo.ToeicPolicyScore);
                    }
                }

                epaXmlBuilder.Append(Constants.EndRootTag);
                return epaXmlBuilder.ToString();
            }
        }

        public override bool IsValid() => ModifiedBy > 0 && !string.IsNullOrWhiteSpace(EPAScores);

        public override string GetSettingKey() => "EPA_UpdateFinalScoreEndEvaluation";
    }
}