namespace NexGoal.Model
{
    using System;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Utilities;

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

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int StatusId { get; set; }

        public string StatusToString
        {
            get
            {
                var status = string.Empty;

                switch (StatusId)
                {
                    case (int)NominationSettingStatus.NotReady:
                        status = Enumerations.GetDescription(NominationSettingStatus.NotReady);
                        break;

                    case (int)NominationSettingStatus.InProgress:
                        status = Enumerations.GetDescription(NominationSettingStatus.InProgress);
                        break;

                    case (int)NominationSettingStatus.Closed:
                        status = Enumerations.GetDescription(NominationSettingStatus.Closed);
                        break;
                }

                return status;
            }
        }

        public string Description { get; set; }
    }
}