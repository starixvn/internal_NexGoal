namespace NexGoal.Criteria.PAFocus
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using NexGoal.Core.Constants;
    using NexGoal.Model;

    public class UpdatePAFocusProcessSetingCriteria : CriteriaBase
    {
        private const string XMLTemplate = "<PAFocusProcessSetting ProcessId='{0}' Duration='{1}'></PAFocusProcessSetting>";

        public int PeriodId { get; set; }

        [SpParam(Ignored = true)]
        public IEnumerable<FocusProcessSettingModel> Processes { get; set; }

        public string PAFocusProcessSettings
        {
            get
            {
                var epaXmlBuilder = new StringBuilder(Constants.BeginRootTag);

                if (Processes != null)
                {
                    foreach (FocusProcessSettingModel item in Processes)
                    {
                        epaXmlBuilder.AppendFormat(
                            XMLTemplate,
                            item.ProcessId,
                            item.Duration);
                    }
                }

                epaXmlBuilder.Append(Constants.EndRootTag);

                return epaXmlBuilder.ToString();
            }
        }

        public override bool IsValid()
        {
            return PeriodId > 0 && Processes != null && Processes.Any();
        }

        public override string GetSettingKey()
        {
            return "PAFocusProcessSetting_Update";
        }
    }
}