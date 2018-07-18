namespace NexGoal.Models.Discipline
{
    using System;

    public class DisciplineModel
    {
        public int DisciplineId { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public DateTime EffectiveDate { get; set; }

        public string Group { get; set; }

        public int EmployeeId { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public int CreatedBy { get; set; }

        public string CreatorNickName { get; set; }

        public string CreatorFullName { get; set; }
    }
}