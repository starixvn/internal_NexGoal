namespace NexGoal.Models.Discipline
{
    using System;

    public class AddDisciplineModel
    {
        public int EmployeeId { get; set; }

        public DateTime EffectiveDate { get; set; }

        public string Content { get; set; }

        public int CreatedBy { get; set; }
    }
}