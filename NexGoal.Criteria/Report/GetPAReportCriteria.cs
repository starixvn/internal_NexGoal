namespace NexGoal.Criteria.Report
{
    using Fanex.Data.Repository;
    using NexGoal.Core.Attribute;

    public class GetPAReportCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public int? EmployeeId { get; set; }

        public int? PeriodId { get; set; }

        public int? DepartmentId { get; set; }

        public int? PageIndex { get; set; }

        public int? ItemsPerPage { get; set; }

        [Output]
        public int? TotalItem { get; set; }

        public override string GetSettingKey()
        {
            return "Report_PAReport";
        }

        public override bool IsValid()
        {
            return UserId > 0;
        }
    }
}