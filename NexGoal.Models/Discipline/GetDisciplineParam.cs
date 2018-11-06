namespace NexGoal.Models.Discipline
{
    using System;

    public class GetDisciplineParam
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int? EmployeeId { get; set; }
        public string GroupIds { get; set; }
        public bool? IsActive { get; set; }
    }
}