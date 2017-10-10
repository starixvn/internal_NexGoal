namespace NexGoal.Model.Dashboard
{
    using System;
    using Newtonsoft.Json;

    public class NominationModel
    {
        public int EOQYId { get; set; }

        [JsonIgnore]
        public int Year { get; set; }

        [JsonIgnore]
        public int Quarter { get; set; }

        public bool IsQuarter { get; set; }

        public int RoleId { get; set; }

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

        public DateTime EndDate { get; set; }
    }
}