namespace NexGoal.Criteria.Report
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetPARecordForReportCriteria : CriteriaBase
    {
        public int ActorId { get; set; }
        public string PeriodIds { get; set; }
        public string GroupIds { get; set; }
        public string EmployeeIds { get; set; }
        public bool? IsRevised { get; set; }

        [SpParam(Ignored = true)]
        public bool IsExportExcel { get; set; }

        public override string GetSettingKey() => IsExportExcel ? "PAReport_GetDataForExportExcel" : "Report_GetPARecord";

        public override bool IsValid() => ActorId > 0;
    }
}