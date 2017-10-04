namespace NexGoal.Model
{
    using Core.Enumerations;
    using System.Collections.Generic;
    using System.Linq;

    public class NominatorSettingModel
    {
        public EOQYModel Period { get; set; }

        public NomineeModel NominatorInfo { get; set; }

        public string RoleIds { get; set; }

        public IEnumerable<int> Roles
        {
            get
            {
                int role;
                return !string.IsNullOrWhiteSpace(RoleIds) ? RoleIds.Split(',').Select(x => int.TryParse(x, out role) ? role : 0) : new List<int>();
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