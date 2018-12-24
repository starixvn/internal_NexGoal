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
                var title = $"Employee of Year - {Year}";

                if (Quarter.HasValue)
                {
                    switch (Quarter)
                    {
                        case 1:
                            title = $"Employee of Quarter - I {Year}";
                            break;

                        case 2:
                            title = $"Employee of Quarter - II {Year}";
                            break;

                        case 3:
                            title = $"Employee of Quarter - III {Year}";
                            break;

                        default:
                            title = $"Employee of Quarter - IV {Year}";
                            break;
                    }
                }

                return title;
            }
        }
    }
}