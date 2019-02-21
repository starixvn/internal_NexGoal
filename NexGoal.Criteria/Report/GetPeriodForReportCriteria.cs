namespace NexGoal.Criteria.Report
{
    using Fanex.Data.Repository;

    public class GetPeriodForReportCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Report_GetPeriods";

        public override bool IsValid() => true;
    }
}