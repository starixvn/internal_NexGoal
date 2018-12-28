namespace NexGoal.AutoService.Models.Nomination
{
    using System;

    public class NominationPeriodModel
    {
        public int NominationPeriodId { get; set; }

        public int? Quarter { get; set; }

        public int Year { get; set; }

        public DateTime StartDateForNominator { get; set; }

        public DateTime EndDateForNominator { get; set; }

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
    }
}