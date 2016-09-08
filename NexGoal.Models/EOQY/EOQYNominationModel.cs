namespace NexGoal.Model
{
    using System.Collections.Generic;
    using NexGoal.Core.Enumerations;
    using NexGoal.Model.Employee;

    public class EOQYNominationModel
    {
        public int NominationId { get; set; }

        public EOQYModel Period { get; set; }

        public EmployeeModel Nominee { get; set; }

        public EmployeeModel Nominator { get; set; }

        public List<EOQYQuestionModel> NominationDetails { get; set; }

        public int StatusId { get; set; }

        public int RoleId { get; set; }

        public int PermissionOnAction
        {
            get
            {
                int permission = (int)Policy.Read;

                switch (StatusId)
                {
                    case (int)NominationStatus.Nominated:
                        if (RoleId == (int)NominationRole.Nominator)
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
                        if (RoleId == (int)NominationRole.NominationBoard)
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