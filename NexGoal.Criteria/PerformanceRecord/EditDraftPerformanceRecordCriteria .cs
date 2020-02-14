namespace NexGoal.Criteria.PerformanceRecord
{
    using NexGoal.Core.Criteria;

    public class EditDraftPerformanceRecordCriteria : CommandBaseCriteria
    {
        public int PerformanceRecordId { get; set; }
        public string EmployeeIds { get; set; }
        public int CreatorId { get; set; }
        public string GradeId { get; set; }
        public string Comment { get; set; }
        public string ProjectIds { get; set; }
        public byte TypeId { get; set; }

        public override string GetSettingKey() => "PerformanceRecord_UpdateDraft";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(EmployeeIds) && CreatorId > 0 && !string.IsNullOrWhiteSpace(Comment);
    }
}