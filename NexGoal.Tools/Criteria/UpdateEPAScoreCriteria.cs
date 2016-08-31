namespace NexGoal.Tools
{
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
        public EPAViewModel EpaInfo { get; set; }

        public string EpaInfoXml
        {
            get
            {
                var epaInfoXml = new StringBuilder(BeginRootTag);

                foreach (EPAModel epa in EpaInfo.Epas)
                {
                    epaInfoXml.AppendFormat(EPAXmlTemplate, epa.EPAId, epa.FinalScore, epa.Grade);
                }

                epaInfoXml.Append(EndRootTag);

                return epaInfoXml.ToString();
            }
        }

        public string ProcessDetailXml
        {
            get
            {
                var epaProcessXml = new StringBuilder(BeginRootTag);

                foreach (EPAProcessModel process in EpaInfo.Processes)
                {
                    epaProcessXml.AppendFormat(EPAProcessXmlTemplate, process.EPAId, process.ProcessId, process.EvaluatorId, process.TotalScore);
                }

                epaProcessXml.Append(EndRootTag);

                return epaProcessXml.ToString();
            }
        }

        public string EpaGroupDetailXml
        {
            get
            {
                var epaGroupXml = new StringBuilder(BeginRootTag);

                foreach (EPAGroupDetailModel gr in EpaInfo.EpaGroups)
                {
                    epaGroupXml.AppendFormat(EPAGroupXmlTemplate, gr.EPAId, gr.ProcessId, gr.EvaluatorId, gr.KPIGroupId, gr.Score);
                }

                epaGroupXml.Append(EndRootTag);

                return epaGroupXml.ToString();
            }
        }

        public string EpaDetailXml
        {
            get
            {
                var epaDetailXml = new StringBuilder(BeginRootTag);

                foreach (EPADetailModel dt in EpaInfo.EpaDetails)
                {
                    epaDetailXml.AppendFormat(EPADetailXmlTemplate, dt.EPAId, dt.ProcessId, dt.EvaluatorId, dt.KPIGroupId, dt.KPIId, dt.Score, dt.Rate);
                }

                epaDetailXml.Append(EndRootTag);

                return epaDetailXml.ToString();
            }
        }

        public override string GetSettingKey()
        {
            return "NexGoalTools_UpdateEPAScore";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}