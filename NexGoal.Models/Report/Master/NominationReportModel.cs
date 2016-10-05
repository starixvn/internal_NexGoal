namespace NexGoal.Model.Report
{
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Utilities;
    using NexGoal.Model.Nomination;
    using System.Collections.Generic;

    public class NominationReportModel
    {
        public NominationReportModel()
        {
            Nominators = new List<NominatorReportModel>();
        }

        public int NomineeId { get; set; }

        public int Quarter { get; set; }

        public int Year { get; set; }

        public int StatusId { get; set; }

        public List<NominatorReportModel> Nominators { get; set; }

        public string Title
        {
            get
            {
                var title = string.Empty;
                switch (Quarter)
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
                    case 4:
                        title = "EOQ: 4th" + Year.ToString();
                        break;
                    default:
                        title = "EOY: " + Year.ToString();
                        break;
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
