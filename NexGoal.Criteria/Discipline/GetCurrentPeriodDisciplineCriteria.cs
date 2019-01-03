namespace NexGoal.Criteria.Discipline
{
    using Fanex.Data.Repository;

    public class GetCurrentPeriodDisciplineCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "Discipline_GetCurrentPeriodByEmployee";

        public override bool IsValid() => EmployeeId > 0;
    }
}