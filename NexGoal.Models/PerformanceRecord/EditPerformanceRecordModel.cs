namespace NexGoal.Models.PerformanceRecord
{
    using System.ComponentModel.DataAnnotations;
    using NexGoal.Resources;

    public class EditPerformanceRecordModel
    {
        public int PerformanceRecordId { get; set; }
        public int EmployeeId { get; set; }
        public int CreatorId { get; set; }
        public string GradeId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.RequiredErrorMessage))]
        public string Comment { get; set; }

        public string ProjectIds { get; set; }
        public int StatusId { get; set; }
        public int RoleId { get; set; }
    }
}