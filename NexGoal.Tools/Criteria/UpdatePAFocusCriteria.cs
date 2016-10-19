namespace NexGoal.Tools
{
    using Fanex.Data.Repository;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class UpdatePAFocusCriteria : CriteriaBase
    {
        private const string XMLTemplate = @"<PAFocus PAFocusId='{0}' IsReadyPromotion='{1}' IsPoorPerformance='{2}' Contribution='{3}' BonusGiven='{4}'></PAFocus>";
        
        [SpParam(Ignored = true)]
        public IEnumerable<PAFocusModel> Evaluations { get; set; }

        public string PAFocusXML
        {
            get
            {
                var epaXmlBuilder = new StringBuilder("<ROOT>");

                if (Evaluations != null)
                {
                    foreach (PAFocusModel item in Evaluations)
                    {
                        epaXmlBuilder.AppendFormat(
                            XMLTemplate,
                            item.PAFocusId,
                            item.EncryptedIsReadyPromotion,
                            item.EncryptedIsPoorPerformance,
                            item.EncryptedContribution,
                            item.EncryptedBonusGiven);
                    }
                }

                epaXmlBuilder.Append("</ROOT>");

                return epaXmlBuilder.ToString();
            }
        }

        public override bool IsValid()
        {
            return Evaluations != null && Evaluations.Any();
        }

        public override string GetSettingKey()
        {
            return "NexGoalTools_UpdatePAFocus";
        }
    }
}