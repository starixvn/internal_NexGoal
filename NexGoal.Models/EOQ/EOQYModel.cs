namespace NexGoal.Model
{
    using System;

    public class EOQYModel
    {
        public int EOQYId { get; set; }

        public int Year { get; set; }

        public int? Quarter { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int StatusId { get; set; }
    }
}