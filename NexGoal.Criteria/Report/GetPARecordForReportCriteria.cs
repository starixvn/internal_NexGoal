namespace NexGoal.Criteria.Report
{
    using Fanex.Data.Repository;

    public class GetPARecordForReportCriteria : CriteriaBase
    {
        public int ActorId { get; set; }

        public string PeriodIds { get; set; }

        public string GroupIds { get; set; }

        public string EmployeeIds { get; set; }

        public override string GetSettingKey() => "Report_GetPARecord";

        public override bool IsValid() => ActorId > 0;
    }
}