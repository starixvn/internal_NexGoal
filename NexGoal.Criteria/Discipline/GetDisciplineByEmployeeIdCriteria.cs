namespace NexGoal.Criteria.Discipline
{
    using Fanex.Data.Repository;

    public class GetDisciplineByEmployeeIdCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }
        public int? PeriodId { get; set; }

        public override string GetSettingKey() => "Discipline_GetByEmployeeId";

        public override bool IsValid() => EmployeeId > 0;
    }
}