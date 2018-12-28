namespace NexGoal.Models.NominationPeriod
{
    using System;

    public class NominationPeriodModel
    {
        public int NominationPeriodId { get; set; }

        public int? Quarter { get; set; }

        public int Year { get; set; }

        public DateTime StartDateForNominator { get; set; }

        public DateTime EndDateForNominator { get; set; }

        public DateTime RemindDateForNominator { get; set; }

        public DateTime EndDateForCommittee { get; set; }

        public string Title
        {
            get
            {
                var title = $"Employee of the Year - {Year}";

                if (Quarter.HasValue)
                {
                    switch (Quarter)
                    {
                        case 1:
                            title = $"Employee of the Quarter - I {Year}";
                            break;

                        case 2:
                            title = $"Employee of the Quarter - II {Year}";
                            break;

                        case 3:
                            title = $"Employee of the Quarter - III {Year}";
                            break;

                        default:
                            title = $"Employee of the Quarter - IV {Year}";
                            break;
                    }
                }

                return title;
            }
        }

        public string ShortTitle
        {
            get
            {
                var title = $"EOY - {Year}";

                if (Quarter.HasValue)
                {
                    switch (Quarter)
                    {
                        case 1:
                            title = $"EOQ - I {Year}";
                            break;

                        case 2:
                            title = $"EOQ - II {Year}";
                            break;

                        case 3:
                            title = $"EOQ - III {Year}";
                            break;

                        default:
                            title = $"EOQ - IV {Year}";
                            break;
                    }
                }

                return title;
            }
        }
    }
}