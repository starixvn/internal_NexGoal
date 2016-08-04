namespace NexGoal.Criteria.Period
{
    using Fanex.Data.Repository;

    public class GetInprogressSemiPeriodCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey()
        {
            return "Period_GetInprogressSemiPeriod";
        }

        public override bool IsValid()
        {
            return EmployeeId > 0;
        }
    }
}
