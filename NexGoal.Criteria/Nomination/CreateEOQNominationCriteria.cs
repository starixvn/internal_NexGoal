namespace NexGoal.Criteria
{
    using Fanex.Data.Repository;
    using NexGoal.Core.Constants;
    using NexGoal.Model;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web;

    public class CreateEOQNominationCriteria : CriteriaBase
    {
        private const string NominationDetailXmlTemp = "<Question QuestionId='{0}' Comment='{1}'></Question>";

        public int EOQYId { get; set; }

        public int NomineeId { get; set; }

        public int NominatorId { get; set; }

        public int StatusId { get; set; }

        public int ModifiedBy { get; set; }

        [SpParam(Ignored = true)]
        public List<QuestionModel> NominationDetails { get; set; }

        [SpParam(SqlParamName = "NominationDetails")]
        public string Details
        {
            get
            {
                var xmlTemp = new StringBuilder(Constants.BeginRootTag);

                if (NominationDetails != null && NominationDetails.Any())
                {
                    foreach (QuestionModel nomination in NominationDetails)
                    {
                        xmlTemp.AppendFormat(NominationDetailXmlTemp, nomination.QuestionId, HttpUtility.HtmlEncode(nomination.Comment));
                    }
                }

                xmlTemp.Append(Constants.EndRootTag);

                return xmlTemp.ToString();
            }
        }

        public override string GetSettingKey()
        {
            return "EOQNomination_Insert";
        }

        public override bool IsValid()
        {
            return EOQYId > 0 && NomineeId > 0 && NominatorId > 0 && StatusId > 0 && ModifiedBy > 0;
        }
    }
}