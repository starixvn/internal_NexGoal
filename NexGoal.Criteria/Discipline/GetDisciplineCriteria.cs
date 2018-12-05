namespace NexGoal.Criteria.Discipline
{
    using System;
    using Fanex.Data.Repository;

    public class GetDisciplineCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int? GroupId { get; set; }
        public int? EmployeeId { get; set; }
        public bool? IsActive { get; set; }

        public override string GetSettingKey() => "Discipline_Get";

        public override bool IsValid() => true;
    }
}