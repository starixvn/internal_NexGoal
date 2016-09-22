namespace NexGoal.Model.Report
{
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Utilities;
    using NexGoal.Resources;

    public class NominationReportModel
    {
        public int NomineeId { get; set; }

        public int? Quarter { get; set; }

        public int Year { get; set; }

        public int StatusId { get; set; }

        public string Title
        {
            get
            {
                var title = string.Empty;

                if (Quarter.HasValue)
                {
                    switch (Quarter.Value)
                    {
                        case 1:
                            title = "EOQ: 1st" + Year.ToString();
                            break;
                        case 2:
                            title = "EOQ: 2nd" + Year.ToString();
                            break;
                        case 3:
                            title = "EOQ: 3rd" + Year.ToString();
                            break;
                        default:
                            title = "EOQ: 4th" + Year.ToString();
                            break;
                    }
                }
                else
                {
                    title = "EOY: " + Year.ToString();
                }

                return title;
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
    }
}
