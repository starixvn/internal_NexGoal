using System;

namespace NexGoal.Model.Dashboard
{
    public class NominationModel
    {
        public int EOQYId { get; set; }

        public int Year { get; set; }

        public int Quarter { get; set; }

        public string RoleIds { get; set; }

        public string Title
        {
            get
            {
                var title = string.Format("EOY {0}", Year);

                if (Quarter > 0)
                {
                    switch (Quarter)
                    {
                        case 1:
                            title = string.Format("Employee of 1st Quarter, {0}", Year);
                            break;

                        case 2:
                            title = string.Format("Employee of 2nd Quarter, {0}", Year);
                            break;

                        case 3:
                            title = string.Format("Employee of 3rd Quarter, {0}", Year);
                            break;

                        default:
                            title = string.Format("Employee of 4th Quarter, {0}", Year);
                            break;
                    }
                }

                return title;
            }
        }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime StartDateForCommittee
        {
            get { return EndDate.AddDays(1); }
        }

        public DateTime EndDateForCommittee { get; set; }
    }
}