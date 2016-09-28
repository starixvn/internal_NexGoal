namespace NexGoal.Model
{
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Utilities;

    public class NominationEPAModel
    {
        public int EOQYId { get; set; }

        public int Year { get; set; }

        public string Quarter { get; set; }

        public string Title
        {
            get
            {
                var title = string.Empty;

                if (!string.IsNullOrEmpty(Quarter))
                {
                    switch (Quarter)
                    {
                        case "1":
                            title = "1st Quarter, " + Year.ToString();
                            break;
                        case "2":
                            title = "2nd Quarter, " + Year.ToString();
                            break;
                        case "3":
                            title = "3rd Quarter, " + Year.ToString();
                            break;
                        default:
                            title = "4th Quarter, " + Year.ToString();
                            break;
                    }
                }
                else
                {
                    title = Year.ToString();
                }

                return title;
            }
        }

        public int StatusId { get; set; }

        public string StatusToString
        {
            get
            {
                return Enumerations.GetDescription((NominationStatus)StatusId);
            }
        }

        public int NumberVotes { get; set; }
    }
}