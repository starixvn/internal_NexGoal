namespace NexGoal.Criteria.EmployeeEPA
{
    using System.Text;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using NexGoal.Core.Enumerations;
    using NexGoal.Model.EmployeeEPA;

    public class ResetScoreCriteria : CriteriaBase
    {
        private const string EPAXmlTemplate = "<EPA ProcessId='{0}' TotalScore='{1}' AdjustScore='{2}'></EPA>";
        private const string BeginRootTag = "<ROOT>";
        private const string EndRootTag = "</ROOT>";

        public int EPAId { get; set; }

        public int ModifiedBy { get; set; }

        [SpParam(Ignored = true)]
        public EPAModel EPAInfo { get; set; }

        public string Details
        {
            get
            {
                var epaXml = new StringBuilder(BeginRootTag);

                epaXml.AppendFormat(
                    EPAXmlTemplate,
                    (int)ProcessType.Reviewer,
                    string.Empty,
                    string.Empty);

                epaXml.AppendFormat(
                    EPAXmlTemplate,
                    (int)ProcessType.Management,
                    string.Empty,
                    string.Empty);

                epaXml.Append(EndRootTag);

                return epaXml.ToString();
            }
        }

        public override bool IsValid()
        {
            return EPAId > 0 && ModifiedBy > 0;
        }

        public override string GetSettingKey()
        {
            return "EPA_ResetScore";
        }
    }
}