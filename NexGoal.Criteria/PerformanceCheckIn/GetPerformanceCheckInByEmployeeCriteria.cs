namespace NexGoal.Criteria.PerformanceCheckIn
{
    using System;
    using Fanex.Data.Repository;

    public class GetPerformanceCheckInByEmployeeCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string CreatorIds { get; set; }
        public string ProjectIds { get; set; }
        public int? PageSize { get; set; }
        public int? PageIndex { get; set; }
        public int TotalItems { get; set; }

        public override string GetSettingKey() => "PerformanceCheckIn_GetByEmployee";

        public override bool IsValid() => EmployeeId > 0;
    }
}