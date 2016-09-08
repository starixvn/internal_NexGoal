namespace NexGoal.Criteria
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.Data.Repository;
    using NexGoal.Core.Constants;
    using NexGoal.Model;

    public class UpdateEOYNominatorSettingByIdCriteria : CriteriaBase
    {
        private const string NominatorSettingXmlTemp = "<NominatorSetting RoleId='{0}' NominatorId='{1}' />";

        public int EOQYId { get; set; }

        [SpParam(Ignored = true)]
        public List<EOQYNominatorSettingModel> Nominators { get; set; }

        public string Details
        {
            get
            {
                var settingXml = new StringBuilder(Constants.BeginRootTag);

                if (Nominators != null && Nominators.Any())
                {
                    foreach (EOQYNominatorSettingModel nominator in Nominators)
                    {
                        settingXml.AppendFormat(NominatorSettingXmlTemp, nominator.RoleId, nominator.NominatorInfo.EmployeeId);
                    }
                }

                settingXml.Append(Constants.EndRootTag);

                return settingXml.ToString();
            }
        }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "EOY_UpdateNominatorSettingById";
        }

        public override bool IsValid()
        {
            return EOQYId > 0 && ModifiedBy > 0;
        }
    }
}