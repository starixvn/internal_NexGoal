namespace NexGoal.Criteria.Report
{
    using Fanex.Data.Repository;

    public class GetHRMByEmployeeInPeriod : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public int PeriodId { get; set; }

        public override string GetSettingKey() => "Report_GetHRM";

        public override bool IsValid() => EmployeeId > 0 && PeriodId > 0;
    }
}