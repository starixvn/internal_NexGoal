namespace NexGoal.Models.PerformanceRecord
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using NexGoal.Core.Enumerations;
    using NexGoal.Models.Common;
    using NexGoal.Models.NominationPeriod;

    public class PerformanceRecordModel
    {
        public int PerformanceRecordId { get; set; }

        [JsonIgnore]
        public int? PeriodId { get; set; }
        public PeriodBaseModel Period { get; set; }

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
        public EmployeeBaseModel Creator { get; set; } = new EmployeeBaseModel();

        public int RoleId { get; set; }

        public int? NominationStatusId { get; set; }

        [JsonIgnore]
        public int? NominationPeriodId { get; set; }
        public NominationPeriodModel NominationPeriod { get; set; }

        public int TypeId { get; set; }

        public bool IsHost => StatusId == PerformanceRecordStatus.Draft || StatusId == PerformanceRecordStatus.Submitted;
    }
}