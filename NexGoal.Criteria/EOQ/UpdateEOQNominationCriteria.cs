namespace NexGoal.Criteria
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.Data.Repository;
    using NexGoal.Core.Constants;
    using NexGoal.Model;

    public class UpdateEOQNominationCriteria : CriteriaBase
    {
        private const string NominationDetailXmlTemp = "<Question QuestionId='{0}' Comment='{1}'></Question>";

        public int NominationId { get; set; }

        public int StatusId { get; set; }

        public int ModifiedBy { get; set; }

        public List<NominationDetailModel> NominationDetails { get; set; }

        [SpParam(SqlParamName = "NominationDetails")]
        public string Details
        {
            get
            {
                var xmlTemp = new StringBuilder(Constants.BeginRootTag);

                if (NominationDetails != null && NominationDetails.Any())
                {
                    foreach (NominationDetailModel nomination in NominationDetails)
                    {
                        xmlTemp.AppendFormat(NominationDetailXmlTemp, nomination.Question.QuestionId, nomination.Question.Comment);
                    }
                }

                xmlTemp.Append(Constants.EndRootTag);

                return xmlTemp.ToString();
            }
        }

        public override string GetSettingKey()
        {
            return "EOQNomination_Update";
        }

        public override bool IsValid()
        {
            return NominationId > 0 && StatusId > 0 && ModifiedBy > 0;
        }
    }
}