namespace NexGoal.Criteria.PAFocus
{
    using Fanex.Data.Repository;
    using NexGoal.Core.Constants;
    using NexGoal.Model.PAFocus;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class UpdatePAFocusCriteria : CriteriaBase
    {
        private const string XMLTemplate = "<PAFocus PAFocusId='{0}' LastObjective='{1}' NextObjective='{2}' "
        + "IsReadyPromotion='{3}' Contribution='{4}' BonusGiven='{5}' BonusRanking='{6}'></PAFocus>";

        public int ModifiedBy { get; set; }

        [SpParam(Ignored = true)]
        public IEnumerable<PAFocusModel> Evaluations { get; set; }

        public string PAFocusXML
        {
            get
            {
                var epaXmlBuilder = new StringBuilder(Constants.BeginRootTag);

                if (Evaluations != null)
                {
                    foreach (PAFocusModel item in Evaluations)
                    {
                        item.Encrypt();
                        epaXmlBuilder.AppendFormat(
                            XMLTemplate,
                            item.PAFocusId,
                            item.LastCompletedPercentObjective,
                            item.NextDescriptionObjective,
                            item.EncryptIsReadyPromotion,
                            item.EncryptedContribution,
                            item.EncryptedBonusGiven,
                            item.EncryptedBonusRanking);
                    }
                }

                epaXmlBuilder.Append(Constants.EndRootTag);

                return epaXmlBuilder.ToString();
            }
        }

        public override bool IsValid()
        {
            return ModifiedBy > 0 && Evaluations != null && Evaluations.Any();
        }

        public override string GetSettingKey()
        {
            return "PAFocus_Update";
        }
    }
}