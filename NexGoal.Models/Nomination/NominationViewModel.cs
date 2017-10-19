namespace NexGoal.Model
{
    using System.Collections.Generic;
    using Core.Enumerations;
    using Core.Utilities;

    public class NominationViewModel
    {
        public EOQYModel Period { get; set; }

        public NomineeModel Nominee { get; set; }

        public List<NominationDetailViewModel> Nominations { get; set; }

        public int StatusId { get; set; }

        public int RoleId { get; set; }

        public int NumberOfNominators
        {
            get
            {
                return Nominations.Count;
            }
        }

        public string StatusToString
        {
            get
            {
                var status = string.Empty;

                switch (StatusId)
                {
                    case (int)NominationStatus.Nominated:
                        status = Enumerations.GetDescription(NominationStatus.Nominated);
                        break;

                    case (int)NominationStatus.Shortlisted:
                        status = Enumerations.GetDescription(NominationStatus.Shortlisted);
                        break;

                    case (int)NominationStatus.Winner:
                        status = Enumerations.GetDescription(NominationStatus.Winner);
                        break;
                }

                return status;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return RoleId == (int)NominationRole.Nominator
                    ? Period.StatusId != (int)NominationSettingStatus.InProgress
                    : Period.StatusId != (int)NominationSettingStatus.Closed;
            }
        }
    }
}