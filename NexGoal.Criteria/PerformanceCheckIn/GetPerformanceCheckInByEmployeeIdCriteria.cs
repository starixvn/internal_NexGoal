namespace NexGoal.Criteria.PerformanceCheckIn
{
    using System;
    using Fanex.Data.Repository;

    public class GetPerformanceCheckInByEmployeeIdCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? CreatorId { get; set; }
        public string ProjectIds { get; set; }
        public int? PageSize { get; set; }
        public int? PageIndex { get; set; }

        public override string GetSettingKey() => "PerformanceCheckIn_GetByEmployee";

        public override bool IsValid() => EmployeeId > 0;
    }
}