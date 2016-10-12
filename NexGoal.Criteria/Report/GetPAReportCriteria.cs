namespace NexGoal.Criteria.Report
{
    using Fanex.Data.Repository;

    public class GetPAReportCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public int? EmployeeId { get; set; }

        public string PeriodIds { get; set; }

        public string GroupIds { get; set; }

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