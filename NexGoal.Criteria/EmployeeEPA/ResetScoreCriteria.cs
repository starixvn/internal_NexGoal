namespace NexGoal.Criteria.EmployeeEPA
{
    using System.Text;
    using Core.Constants;
    using Core.Enumerations;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Model.EmployeeEPA;

    public class ResetScoreCriteria : CriteriaBase
    {
        private const string EPAXmlTemplate = "<EPA ProcessId='{0}' TotalScore='{1}' AdjustScore='{2}'></EPA>";

        public int EPAId { get; set; }

        public int ModifiedBy { get; set; }

        [SpParam(Ignored = true)]
        public EPAModel EPAInfo { get; set; }

        public static string Details
        {
            get
            {
                var epaXml = new StringBuilder(Constants.BeginRootTag);

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

                epaXml.Append(Constants.EndRootTag);

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