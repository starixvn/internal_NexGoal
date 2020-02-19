namespace NexGoal.Models.PerformanceRecord
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using NexGoal.Models.Common;
    using NexGoal.Models.NominationPeriod;
    using NexGoal.Models.PRFeedback;
    using NexGoal.Models.PRReaction;

    public class PerformanceRecord
    {
        public int PerformanceRecordId { get; set; }
        public PeriodBaseModel Period { get; set; }

        [JsonIgnore]
        public string GradeId { get; set; }
        public PerformanceGrade Grade { get; set; }

        public string Comment { get; set; }
        public bool IsDraft { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public IEnumerable<ProjectModel> Projects { get; set; } = new List<ProjectModel>();
        public IEnumerable<Employee> Employees { get; set; } = new List<Employee>();
        public Employee Creator { get; set; }
        public int RoleId { get; set; }

        public int? NominationStatusId { get; set; }
        public NominationPeriodModel NominationPeriod { get; set; }

        public byte TypeId { get; set; }
        public bool IsHost { get; set; }

        public bool IsInCurrentPeriod { get; set; }

        public PASummaryModel PASummary { get; set; }
        public IEnumerable<PRReactionModel> Reactions { get; set; } = new List<PRReactionModel>();
        public IEnumerable<PRFeedbackModel> Feedbacks { get; set; } = new List<PRFeedbackModel>();
    }
}