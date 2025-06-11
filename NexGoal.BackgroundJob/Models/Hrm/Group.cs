namespace NexGoal.BackgroundJob.Models.Hrm
{
    using System;

    public class Group
    {
        public int GroupId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int? DepartmentId { get; set; }
        public int? Rank { get; set; }
        public bool? IsDeleted { get; set; }
        public int? ParentId { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}