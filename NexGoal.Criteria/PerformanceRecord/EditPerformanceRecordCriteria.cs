namespace NexGoal.Criteria.PerformanceRecord
{
    public class EditPerformanceRecordCriteria : CommandBaseCriteria
    {
        public int PerformanceRecordId { get; set; }
        public int EmployeeId { get; set; }
        public int CreatorId { get; set; }
        public string GradeId { get; set; }
        public string Comment { get; set; }
        public string ProjectIds { get; set; }
        public int StatusId { get; set; }
        public int TypeId { get; set; }

        public override string GetSettingKey() => "PerformanceRecord_Update";

        public override bool IsValid() => PerformanceRecordId > 0 && EmployeeId > 0 && CreatorId > 0 && !string.IsNullOrWhiteSpace(Comment);
    }
}