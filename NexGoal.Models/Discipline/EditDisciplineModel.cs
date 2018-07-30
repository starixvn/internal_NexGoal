namespace NexGoal.Models.Discipline
{
    using System;

    public class EditDisciplineModel
    {
        public int DisciplineId { get; set; }

        public DateTime EffectiveDate { get; set; }

        public string Content { get; set; }

        public int ModifiedBy { get; set; }
    }
}