namespace NexGoal.Criteria.PerformanceRecord
{
    using System;
    using Fanex.Data.Repository;

    public class GetLastPerformanceRecordByEmployeeCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }
        public DateTime FromDate { get; set; }  
        public DateTime ToDate { get; set; }

        public override string GetSettingKey() => "PerformanceRecord_GetTheLastByEmployee";

        public override bool IsValid() => EmployeeId > 0 && FromDate <= ToDate;
    }
}
