namespace NexGoal.Criteria.Period
{
    using Fanex.Data.Repository;

    public class GetAllPeriodByEmployeeCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override bool IsValid()
        {
            return EmployeeId > 0;
        }

        public override string GetSettingKey()
        {
            return "Period_GetAllByEmployee";
        }
    }
}