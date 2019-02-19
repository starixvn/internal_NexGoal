namespace NexGoal.Criteria.Periods
{
    using Fanex.Data.Repository;

    public class GetDataToEditFinalPAGradeCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }
        public string PARecordIds { get; set; }

        public override string GetSettingKey() => "Period_GetDataToUpdateFinalPAGrade";

        public override bool IsValid() => PeriodId > 0;
    }
}