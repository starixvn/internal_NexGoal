namespace NexGoal.Criteria.Discipline
{
    public class UpdateDisciplineStatusCriteria : CommandBaseCriteria
    {
        public int DisciplineId { get; set; }

        public bool IsActive { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "Discipline_UpdateStatus";

        public override bool IsValid() => DisciplineId > 0;
    }
}