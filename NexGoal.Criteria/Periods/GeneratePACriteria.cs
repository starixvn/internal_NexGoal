namespace NexGoal.Criteria.Periods
{
    using NexGoal.Core.Criteria;

    public class GeneratePACriteria : CommandBaseCriteria
    {
        public int PeriodId { get; set; }

        public string PARecordIds { get; set; }

        public int CreatedBy { get; set; }

        public bool Override { get; set; }

        public override string GetSettingKey() => "Period_GeneratePA";

        public override bool IsValid() => PeriodId > 0 && !string.IsNullOrEmpty(PARecordIds) && CreatedBy > 0;
    }
}