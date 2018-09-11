namespace NexGoal.Criteria.Periods
{
    using Fanex.Data.Repository;

    public class GetEmployeeSettingsCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        public int? GroupId { get; set; }

        public bool? IsEvaluated { get; set; }

        public int? StatusId { get; set; }

        public override string GetSettingKey() => "Period_GetEmployeeSettings";

        public override bool IsValid() => PeriodId > 0;
    }
}