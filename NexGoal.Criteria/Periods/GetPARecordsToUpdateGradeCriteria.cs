namespace NexGoal.Criteria.Periods
{
    using Fanex.Data.Repository;

    public class GetPARecordsToUpdateGradeCriteria : CriteriaBase
    {
        public string PARecordIds { get; set; }

        public override string GetSettingKey() => "Period_GetPARecordsToUpdateGrade";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(PARecordIds);
    }
}