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
                        case (byte)Core.Enumerations.Quarter.First:
                            title = $"Employee of the Quarter - I {Year}";
                            break;

                        case (byte)Core.Enumerations.Quarter.Second:
                            title = $"Employee of the Quarter - II {Year}";
                            break;

                        case (byte)Core.Enumerations.Quarter.Third:
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
                var title = $"EOY {Year}";

                if (Quarter.HasValue)
                {
                    switch (Quarter)
                    {
                        case (byte)Core.Enumerations.Quarter.First:
                            title = $"EOQ I {Year}";
                            break;

                        case (byte)Core.Enumerations.Quarter.Second:
                            title = $"EOQ II {Year}";
                            break;

                        case (byte)Core.Enumerations.Quarter.Third:
                            title = $"EOQ III {Year}";
                            break;

                        default:
                            title = $"EOQ IV {Year}";
                            break;
                    }
                }

                return title;
            }
        }
    }
}