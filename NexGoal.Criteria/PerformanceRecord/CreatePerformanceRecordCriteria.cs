namespace NexGoal.Criteria.PerformanceRecord
{
    public class CreatePerformanceRecordCriteria : CommandBaseCriteria
    {
        public int PerformanceRecordId { get; set; }
        public int EmployeeId { get; set; }
        public int CreatorId { get; set; }
        public string GradeId { get; set; }
        public string Comment { get; set; }
        public string ProjectIds { get; set; }
        public int StatusId { get; set; }
        public int TypeId { get; set; }

        public override string GetSettingKey() => "PerformanceRecord_Insert";

        public override bool IsValid() => EmployeeId > 0 && CreatorId > 0 && !string.IsNullOrWhiteSpace(Comment);
    }
}