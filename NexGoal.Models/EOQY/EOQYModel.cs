namespace NexGoal.Models.EOQY
{
    using System;
    using NexGoal.Core.Enumerations;

    public class EOQYModel
    {
        public int EOQYId { get; set; }

        public int Year { get; set; }

        public int Quarter { get; set; }

        public string Description { get; set; }

        public DateTime StartDateForNominator { get; set; }

        public DateTime EndDateForNominator { get; set; }

        public DateTime RemindDateForNominator { get; set; }

        public DateTime StartDateForCommittee
        {
            get { return EndDateForNominator.AddDays(1); }
        }

        public DateTime EndDateForCommittee { get; set; }

        public int StatusId
        {
            get
            {
                var today = DateTime.Now.Date;

                if (StartDateForCommittee.Date > today)
                {
                    return (int)NominationSettingStatus.NotReady;
                }
                else if (StartDateForCommittee.Date <= today && EndDateForCommittee.Date >= today)
                {
                    return (int)NominationSettingStatus.InProgress;
                }
                else
                {
                    return (int)NominationSettingStatus.Closed;
                }
            }
        }
    }
}