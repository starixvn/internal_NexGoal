namespace NexGoal.Models.PerformanceRecord
{
    using System.ComponentModel.DataAnnotations;
    using NexGoal.Resources;

    public class CreatePerformanceRecordDto
    {
        public string EmployeeIds { get; set; }
        public int CreatorId { get; set; }
        public string GradeId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.RequiredErrorMessage))]
        public string Comment { get; set; }

        public string ProjectIds { get; set; }
        public int TypeId { get; set; }
    }
}