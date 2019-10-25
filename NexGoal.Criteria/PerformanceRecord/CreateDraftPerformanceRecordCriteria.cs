namespace NexGoal.Criteria.PerformanceRecord
{
    using NexGoal.Core.Criteria;

    public class CreateDraftPerformanceRecordCriteria : CommandBaseCriteria
    {
        public string PerformanceRecordIds { get; set; }
        public string Comment { get; set; }
        public int CreatorId { get; set; }
        public string EmployeeIds { get; set; }
        public string GradeId { get; set; }
        public string ProjectIds { get; set; }
        public int TypeId { get; set; }

        public override string GetSettingKey() => "PerformanceRecord_InsertDraft";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(EmployeeIds) && CreatorId > 0 && !string.IsNullOrWhiteSpace(Comment);
    }
}