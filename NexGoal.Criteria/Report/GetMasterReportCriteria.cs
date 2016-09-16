namespace NexGoal.Criteria.Report
{
    using Fanex.Data.Repository;

    public class GetMasterReportCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        public int? EmployeeId { get; set; }

        public int? GroupIds { get; set; }

        public override string GetSettingKey()
        {
            return "Report_Master";
        }

        public override bool IsValid()
        {
            return PeriodId > 0;
        }
    }
}