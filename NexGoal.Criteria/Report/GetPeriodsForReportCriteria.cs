namespace NexGoal.Criteria.Report
{
    using Fanex.Data.Repository;

    public class GetPeriodsForReportCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Report_GetPeriods";

        public override bool IsValid() => true;
    }
}