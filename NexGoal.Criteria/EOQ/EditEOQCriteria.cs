namespace NexGoal.Criteria.EOQ
{
    using System;

    public class EditEOQCriteria : CommandBaseCriteria
    {
        public int EOQYId { get; set; }
        public int Quarter { get; set; }
        public int Year { get; set; }
        public string Description { get; set; }
        public DateTime StartDateForNominator { get; set; }
        public DateTime EndDateForNominator { get; set; }
        public DateTime RemindDateForNominator { get; set; }
        public DateTime EndDateForCommittee { get; set; }
        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "EOQ_Update";

        public override bool IsValid() => Quarter > 0 && Year > 0 && ModifiedBy > 0;
    }
}