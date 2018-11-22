namespace NexGoal.ViewModels.NominationPeriod
{
    using System.Collections.Generic;
    using System.Linq;
    using NexGoal.Core.Enumerations;
    using NexGoal.Models.NominationPeriod;

    public class RoleSettingViewModel : RoleSettingModel
    {
        public IEnumerable<int> Roles
        {
            get
            {
                IEnumerable<int> roleIds = new List<int>();

                if (!string.IsNullOrWhiteSpace(RoleIds))
                {
                    roleIds = RoleIds.Split(',').Select(x => int.TryParse(x, out int role) ? role : 0);
                }

                return roleIds;
            }
        }

        public bool IsNominator
        {
            get { return Roles.Any(x => x == (int)NominationRole.Nominator); }
        }

        public bool IsCommittee
        {
            get { return Roles.Any(x => x == (int)NominationRole.Committee); }
        }
    }
}