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
                        case (byte)Core.Enumerations.Quarter.First:
                            title = $"EOQ - I {Year}";
                            break;

                        case (byte)Core.Enumerations.Quarter.Second:
                            title = $"EOQ - II {Year}";
                            break;

                        case (byte)Core.Enumerations.Quarter.Third:
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