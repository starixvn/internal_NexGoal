namespace NexGoal.BackgroundService
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Core.Constants;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using NexGoal.Model.Period;

    public class UpdateIsEndProcessCriteria : CriteriaBase
    {
        private const string ProcessXmlTemplate = "<EPAProcess EPAId='{0}' ProcessId='{1}'></EPAProcess>";

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

        private static string ConvertEvaluationProcessToXml(IEnumerable<AutoEmployeePeriodModel> evaluationProcesses)
        {
            var epaProcessXmlBuilder = new StringBuilder(Constants.BeginRootTag);

            foreach (AutoEmployeePeriodModel evaluationProcess in evaluationProcesses)
            {
                epaProcessXmlBuilder.AppendFormat(ProcessXmlTemplate, evaluationProcess.EPAId, evaluationProcess.ProcessId);
            }

            epaProcessXmlBuilder.Append(Constants.EndRootTag);

            return epaProcessXmlBuilder.ToString();
        }
    }
}