namespace NexGoal.Tools
{
    using System.Collections.Generic;
    using System.Text;
    using Fanex.Data.Repository;

    public class UpdateEPAScoreCriteria : CriteriaBase
    {
        private const string BeginRootTag = "<ROOT>";
        private const string EndRootTag = "</ROOT>";
        private const string EPAXmlTemplate = @"<EPA EPAId='{0}' FinalScore='{1}' Grade='{2}'></EPA>";
        private const string EPAProcessXmlTemplate = @"<EPAProcess EPAId='{0}' ProcessId='{1}' EvaluatorId='{2}' TotalScore='{3}'></EPAProcess>";
        private const string EPAGroupXmlTemplate = @"<EPAGroup EPAId='{0}' ProcessId='{1}' EvaluatorId='{2}' KPIGroupId='{3}' Score='{4}'></EPAGroup>";

        private const string EPADetailXmlTemplate = @"<EPADetail EPAId='{0}' ProcessId='{1}' EvaluatorId='{2}'
            KPIGroupId='{3}' KPIId='{4}' Score='{5}' Rate='{6}'></EPADetail>";

        [SpParam(Ignored = true)]
        public List<EPAModel> EpaInfos { get; set; }

        public string EpaInfoXml { get; set; }

        public string ProcessDetailXml { get; set; }

        public string EpaGroupDetailXml { get; set; }

        public string EpaDetailXml { get; set; }

        public override string GetSettingKey()
        {
            return "NexGoalTools_UpdateEPAScore";
        }

        public override bool IsValid()
        {
            return true;
        }

        public void ConvertToXml()
        {
            var epaInfoXml = new StringBuilder(BeginRootTag);
            var epaProcessXml = new StringBuilder(BeginRootTag);
            var epaGroupXml = new StringBuilder(BeginRootTag);
            var epaDetailXml = new StringBuilder(BeginRootTag);

            foreach (EPAModel epaInfo in EpaInfos)
            {
                epaInfoXml.AppendFormat(EPAXmlTemplate, epaInfo.EPAId, epaInfo.FinalScore, epaInfo.Grade);

                foreach (EPAProcessModel process in epaInfo.EPAProcesses)
                {
                    epaProcessXml.AppendFormat(EPAProcessXmlTemplate, process.EPAId, process.ProcessId, process.EvaluatorId, process.TotalScore);

                    foreach (EPAGroupDetailModel gr in process.EPAGroupDetails)
                    {
                        epaGroupXml.AppendFormat(EPAGroupXmlTemplate, gr.EPAId, gr.ProcessId, gr.EvaluatorId, gr.KPIGroupId, gr.Score);

                        foreach (EPADetailModel dt in gr.EPADetails)
                        {
                            epaDetailXml.AppendFormat(EPADetailXmlTemplate, dt.EPAId, dt.ProcessId, dt.EvaluatorId, dt.KPIGroupId, dt.KPIId, dt.Score, dt.Rate);
                        }
                    }
                }
            }

            epaInfoXml.Append(EndRootTag);
            epaProcessXml.Append(EndRootTag);
            epaGroupXml.Append(EndRootTag);
            epaDetailXml.Append(EndRootTag);

            EpaInfoXml = epaInfoXml.ToString();
            ProcessDetailXml = epaProcessXml.ToString();
            EpaGroupDetailXml = epaGroupXml.ToString();
            EpaDetailXml = epaDetailXml.ToString();
        }
    }
}