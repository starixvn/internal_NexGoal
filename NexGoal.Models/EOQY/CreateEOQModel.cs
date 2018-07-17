namespace NexGoal.Models.EOQY
{
    using System;

    public class CreateEOQModel
    {
        public int Year { get; set; }
        public int Quarter { get; set; }
        public string Description { get; set; }
        public DateTime StartDateForNominator { get; set; }
        public DateTime EndDateForNominator { get; set; }
        public DateTime RemindDateForNominator { get; set; }
        public DateTime EndDateForCommittee { get; set; }
    }
}