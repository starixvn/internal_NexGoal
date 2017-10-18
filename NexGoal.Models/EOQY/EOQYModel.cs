namespace NexGoal.Model
{
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Utilities;
    using System;

    public class EOQYModel
    {
        public int EOQYId { get; set; }

        public int Year { get; set; }

        public int Quarter { get; set; }

        public string QuarterToString
        {
            get
            {
                var quarter = string.Empty;

                switch (Quarter)
                {
                    case 1:
                        quarter = "1st Quarter";
                        break;

                    case 2:
                        quarter = "2nd Quarter";
                        break;

                    case 3:
                        quarter = "3rd Quarter";
                        break;

                    case 4:
                        quarter = "4th Quarter";
                        break;
                }

                return quarter;
            }
        }

        public string Title
        {
            get
            {
                var title = "EOY " + Year.ToString();

                if (Quarter != 0)
                {
                    switch (Quarter)
                    {
                        case 1:
                            title = "Employee of 1st Quarter, " + Year.ToString();
                            break;

                        case 2:
                            title = "Employee of 2nd Quarter, " + Year.ToString();
                            break;

                        case 3:
                            title = "Employee of 3rd Quarter, " + Year.ToString();
                            break;

                        default:
                            title = "Employee of 4th Quarter, " + Year.ToString();
                            break;
                    }
                }

                return title;
            }
        }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime RemindDate { get; set; }

        public DateTime StartDateForCommittee
        {
            get { return EndDate.AddDays(1); }
        }

        public DateTime EndDateForCommittee { get; set; }

        public int StatusId
        {
            get
            {
                if (StartDate.Date > DateTime.Now.Date)
                {
                    return (int)NominationSettingStatus.NotReady;
                }
                else if (StartDate.Date <= DateTime.Now.Date && EndDateForCommittee.Date >= DateTime.Now.Date)
                {
                    return (int)NominationSettingStatus.InProgress;
                }

                return (int)NominationSettingStatus.Closed;
            }
        }

        public string StatusToString
        {
            get
            {
                return Enumerations.GetDescription((NominationSettingStatus)StatusId);
            }
        }

        public string Description { get; set; }

        public int StatusIdForNominator
        {
            get
            {
                if (StartDate.Date > DateTime.Now.Date)
                {
                    return (int)NominationSettingStatus.NotReady;
                }
                else if (StartDate.Date <= DateTime.Now.Date && EndDate.Date >= DateTime.Now.Date)
                {
                    return (int)NominationSettingStatus.InProgress;
                }

                return (int)NominationSettingStatus.Closed;
            }
        }

        public int StatusIdForCommittee
        {
            get
            {
                if (StartDateForCommittee.Date > DateTime.Now.Date)
                {
                    return (int)NominationSettingStatus.NotReady;
                }
                else if (StartDateForCommittee.Date <= DateTime.Now.Date && EndDateForCommittee.Date >= DateTime.Now.Date)
                {
                    return (int)NominationSettingStatus.InProgress;
                }

                return (int)NominationSettingStatus.Closed;
            }
        }
    }
}