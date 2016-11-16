namespace NexGoal.BackgroundService
{
    using Fanex.Data.Repository;
    using System.Text;
    using Fanex.Data;

    public class UpdateFinalScoreEndEvaluationCriteria : CriteriaBase
    {
        private const string TemplateXml = "<EPA EPAId='{0}' FinalScore='{1}' DisciplineScore='{2}' Grade='{3}'></EPA>";
        private const string BeginRootTag = "<ROOT>";
        private const string EndRootTag = "</ROOT>";

        [SpParam(SqlParamName = "EPAScores")]
        public string EvaluationScores
        {
            get
            {
                return ConvertToXml(EvalutionId, FinalScore, DisciplineScore, Grade);
            }
        }

        [SpParam(Ignored = true)]
        public int EvalutionId { get; set; }

        [SpParam(Ignored = true)]
        public string FinalScore { get; set; }

        [SpParam(Ignored = true)]
        public double DisciplineScore { get; set; }

        [SpParam(Ignored = true)]
        public string Grade { get; set; }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(EvaluationScores);
        }

        public override string GetSettingKey()
        {
            return "EpaAuto_UpdateFinalScoreEndEvaluation";
        }

        public string ConvertToXml(int evaluationId, string finalScore, double disciplineScore, string grade)
        {
            var epaXmlBuilder = new StringBuilder(BeginRootTag);

            epaXmlBuilder.AppendFormat(TemplateXml, evaluationId, finalScore, disciplineScore, grade);
            epaXmlBuilder.Append(EndRootTag);

            return epaXmlBuilder.ToString();
        }
    }
}