namespace NexGoal.Criteria.Periods
{
    using Fanex.Data.Repository;

    public class GetEmployeeSettingByIdCriteria : CriteriaBase
    {
        public int PARecordId { get; set; }

        public override string GetSettingKey() => "Period_GetEmployeeSetting";

        public override bool IsValid() => PARecordId > 0;
    }
}