namespace NexGoal.Criteria.Discipline
{
    using System;
    using NexGoal.Core.Criteria;

    public class AddDisciplineCriteria : CommandBaseCriteria
    {
        public int EmployeeId { get; set; }

        public DateTime EffectiveDate { get; set; }

        public string Description { get; set; }

        public int CreatedBy { get; set; }

        public override string GetSettingKey() => "Discipline_Insert";

        public override bool IsValid() => EmployeeId > 0 && CreatedBy > 0 && !string.IsNullOrWhiteSpace(Description);
    }
}