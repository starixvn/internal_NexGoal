namespace NexGoal.Criteria.PerformanceRecord
{
    using Fanex.Data.Repository;

    public class GetPerformanceRecordPaginationCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public int Skip { get; set; }

        public int PageSize { get; set; }

        public override string GetSettingKey() => "PerformanceRecord_GetByEmployeePagination";

        public override bool IsValid() => EmployeeId > 0 && PageSize > 0;
    }
}