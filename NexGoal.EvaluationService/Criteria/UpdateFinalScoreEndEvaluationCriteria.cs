namespace NexGoal.BackgroundService
{
    using System.Text;
    using Core.Constants;
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class UpdateFinalScoreEndEvaluationCriteria : CriteriaBase
    {
        private const string TemplateXml = "<EPA EPAId='{0}' FinalScore='{1}' DisciplineScore='{2}' Grade='{3}' ToeicPolicyScore='{4}'></EPA>";

        [SpParam(SqlParamName = "EPAScores")]
        public string EvaluationScores => ConvertToXml();

        [SpParam(Ignored = true)]
        public int EvaluationId { get; set; }

        [SpParam(Ignored = true)]
        public string FinalScore { get; set; }

        [SpParam(Ignored = true)]
        public double DisciplineScore { get; set; }

        [SpParam(Ignored = true)]
        public string Grade { get; set; }

        [SpParam(Ignored = true)]
        public int ToeicPolicyScore { get; set; }

        public override bool IsValid() => !string.IsNullOrWhiteSpace(EvaluationScores);

        public override string GetSettingKey() => "EpaAuto_UpdateFinalScoreEndEvaluation";

        private string ConvertToXml()
        {
            var epaXmlBuilder = new StringBuilder(Constants.BeginRootTag);

            epaXmlBuilder.AppendFormat(TemplateXml, EvaluationId, FinalScore, DisciplineScore, Grade, ToeicPolicyScore);
            epaXmlBuilder.Append(Constants.EndRootTag);

            return epaXmlBuilder.ToString();
        }
    }
}