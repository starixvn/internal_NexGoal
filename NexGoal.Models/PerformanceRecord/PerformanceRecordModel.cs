namespace NexGoal.Models.PerformanceRecord
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using NexGoal.Core.Enumerations;
    using NexGoal.Models.Common;
    using NexGoal.Models.EmployeeNomination;

    public class PerformanceRecordModel
    {
        public int PerformanceRecordId { get; set; }

        [JsonIgnore]
        public string GradeId { get; set; }

        public PerformanceGradeModel Grade { get; set; }

        public string Comment { get; set; }
        public PerformanceRecordStatus StatusId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        [JsonIgnore]
        public string ProjectIds { get; set; }

        public IEnumerable<ProjectModel> Projects { get; set; } = new List<ProjectModel>();

        [JsonIgnore]
        public string EmployeeIds { get; set; }

        public IEnumerable<EmployeeBaseModel> Employees { get; set; } = new List<EmployeeBaseModel>();

        [JsonIgnore]
        public int CreatorId { get; set; }

        public EmployeeBaseModel Creator { get; set; }

        public int RoleId { get; set; }

        public int? NominationId { get; set; }

        public NominationModel Nomination { get; set; }

        public bool IsHost => StatusId == PerformanceRecordStatus.Draft || StatusId == PerformanceRecordStatus.Submitted;
    }
}