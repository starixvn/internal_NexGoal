namespace NexGoal.Model
{
    using System.Collections.Generic;
    using System.Linq;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Utilities;
    using NexGoal.Model.Employee;

    public class EOQYNominationViewModel
    {
        public EOQYModel Period { get; set; }

        public EmployeeModel Nominee { get; set; }

        public List<EmployeeModel> Nominators { get; set; }

        public List<EOQYNominationModel> Nominations { get; set; }

        public int StatusId { get; set; }

        public int RoleId { get; set; }

        public int NumberOfNominators
        {
            get
            {
                return Nominators.Count();
            }
        }

        public string Status2String
        {
            get
            {
                var status = string.Empty;

                switch (StatusId)
                {
                    case (int)NominationStatus.Nominated:
                        Enumerations.GetDescription(NominationStatus.Nominated);
                        break;

                    case (int)NominationStatus.ShortedList:
                        Enumerations.GetDescription(NominationStatus.ShortedList);
                        break;

                    case (int)NominationStatus.Winner:
                        Enumerations.GetDescription(NominationStatus.Winner);
                        break;
                }

                return status;
            }
        }

        public int PermissionOnAction
        {
            get
            {
                int permission = (int)Policy.Read;

                switch (StatusId)
                {
                    case (int)NominationStatus.Nominated:
                        if (RoleId == (int)NominationRole.Nominator && Period.StatusId == (int)NominationSettingStatus.InProgress)
                        {
                            permission = (int)Policy.Host;
                        }

                        break;

                    case (int)NominationStatus.ShortedList:
                    case (int)NominationStatus.Winner:
                        break;
                }

                return permission;
            }
        }

        public int PermissionOnStatus
        {
            get
            {
                int permission = (int)Policy.Read;

                switch (StatusId)
                {
                    case (int)NominationStatus.Nominated:
                        break;

                    case (int)NominationStatus.ShortedList:
                    case (int)NominationStatus.Winner:
                        if (RoleId == (int)NominationRole.NominationBoard && Period.StatusId == (int)NominationSettingStatus.Closed)
                        {
                            permission = (int)Policy.Host;
                        }

                        break;
                }

                return permission;
            }
        }
    }
}