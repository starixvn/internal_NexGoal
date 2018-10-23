namespace NexGoal.Models.PerformanceRecord
{
    using System;
    using System.Collections.Generic;
    using NexGoal.Core.Enumerations;
    using NexGoal.Models.Common;

    public class PerformanceRecordModel
    {
        public int PerformanceRecordId { get; set; }

        public string GradeId { get; set; }
        public PerformanceGradeModel Grade { get; set; }

        public string Comment { get; set; }
        public PerformanceRecordStatus StatusId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public string ProjectIds { get; set; }
        public IEnumerable<ProjectModel> Projects { get; set; } = new List<ProjectModel>();

        public int EmployeeId { get; set; }
        public EmployeeBaseModel Employee { get; set; }

        public int CreatorId { get; set; }
        public EmployeeBaseModel Creator { get; set; }

        public int RoleId { get; set; }
    }
}