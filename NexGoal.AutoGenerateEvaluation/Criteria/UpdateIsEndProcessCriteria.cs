namespace NexGoal.AutoGenerateEvaluation
{
    using Fanex.Data.Repository;
    using NexGoal.Model.Period;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class UpdateIsEndProcessCriteria : CriteriaBase
    {
        private const string ProcessXmlTemplate = "<EPAProcess EPAId='{0}' ProcessId='{1}'></EPAProcess>";
        private const string BeginRootTag = "<ROOT>";
        private const string EndRootTag = "</ROOT>";

        public string EPAProcess
        {
            get
            {
                return ConvertEvaluationProcessToXml(EvaluationProcesses);
            }
        }

        [SpParam(Ignored = true)]
        public IEnumerable<AutoEmployeePeriodModel> EvaluationProcesses { get; set; }

        public override bool IsValid()
        {
            return EvaluationProcesses.Any();
        }

        public override string GetSettingKey()
        {
            return "EpaAuto_UpdateIsEndProcess";
        }

        private string ConvertEvaluationProcessToXml(IEnumerable<AutoEmployeePeriodModel> evaluationProcesses)
        {
            var epaProcessXmlBuilder = new StringBuilder(BeginRootTag);

            foreach (AutoEmployeePeriodModel evaluationProcess in evaluationProcesses)
            {
                epaProcessXmlBuilder.AppendFormat(ProcessXmlTemplate, evaluationProcess.EPAId, evaluationProcess.ProcessId);
            }

            epaProcessXmlBuilder.Append(EndRootTag);

            return epaProcessXmlBuilder.ToString();
        }
    }
}