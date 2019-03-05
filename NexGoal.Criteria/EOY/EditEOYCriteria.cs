namespace NexGoal.Criteria.EOY
{
    using System;
    using NexGoal.Core.Criteria;

    public class EditEOYCriteria : CommandBaseCriteria
    {
        public int EOYId { get; set; }
        public int Year { get; set; }
        public string Description { get; set; }
        public DateTime StartDateForNominator { get; set; }
        public DateTime EndDateForNominator { get; set; }
        public DateTime RemindDateForNominator { get; set; }
        public DateTime EndDateForCommittee { get; set; }
        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "EOY_Update";

        public override bool IsValid() => EOYId > 0 && Year > 0 && ModifiedBy > 0;
    }
}