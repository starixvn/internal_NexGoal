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
                var title = "Employee of Year " + Year.ToString();

                if (!string.IsNullOrEmpty(Quarter))
                {
                    switch (Quarter)
                    {
                        case "1":
                            title = "Employee of 1st Quarter, " + Year.ToString();
                            break;
                        case "2":
                            title = "Employee of 2nd Quarter, " + Year.ToString();
                            break;
                        case "3":
                            title = "Employee of 3rd Quarter, " + Year.ToString();
                            break;
                        default:
                            title = "Employee of 4th Quarter, " + Year.ToString();
                            break;
                    }
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