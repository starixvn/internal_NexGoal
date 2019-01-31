namespace NexGoal.Models.MasterReport
{
    using NexGoal.Core.Enumerations;

    public class AwardModel
    {
        public int NomineeId { get; set; }

        public NominationStatus StatusId { get; set; }

        public string Status { get; set; }

        public int? Quarter { get; set; }

        public int Year { get; set; }

        public string Title
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