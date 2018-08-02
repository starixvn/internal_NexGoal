namespace NexGoal.Criteria.Discipline
{
    using System;

    public class EditDisciplineCriteria : CommandBaseCriteria
    {
        public int DisciplineId { get; set; }

        public DateTime EffectiveDate { get; set; }

        public string Description { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "Discipline_Update";

        public override bool IsValid() => DisciplineId > 0 && ModifiedBy > 0 && !string.IsNullOrWhiteSpace(Description);
    }
}