namespace NexGoal.Models.EmployeeNomination
{
    public class NominationPeriodModel
    {
        public int NominationPeriodId { get; set; }

        public int? Quarter { get; set; }

        public int Year { get; set; }

        public string Title
        {
            get
            {
                var title = $"Employee of the Year, {Year}";

                if (Quarter.HasValue)
                {
                    switch (Quarter)
                    {
                        case 1:
                            title = $"Employee of 1st Quarter, {Year}";
                            break;

                        case 2:
                            title = $"Employee of 2nd Quarter, {Year}";
                            break;

                        case 3:
                            title = $"Employee of 3rd Quarter, {Year}";
                            break;

                        default:
                            title = $"Employee of 4th Quarter, {Year}";
                            break;
                    }
                }

                return title;
            }
        }
    }
}