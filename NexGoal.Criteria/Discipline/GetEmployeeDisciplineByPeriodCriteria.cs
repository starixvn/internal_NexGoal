namespace NexGoal.Criteria.Discipline
{
    using Fanex.Data.Repository;

    public class GetEmployeeDisciplineByPeriodCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public int PeriodId { get; set; }

        public override string GetSettingKey() => "Discipline_GetByEmployeeAndPeriod";

        public override bool IsValid() => EmployeeId > 0 && PeriodId > 0;
    }
}