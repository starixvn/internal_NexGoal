namespace NexGoal.Criteria.Common
{
    using Fanex.Data.Repository;

    public class GetHRMSummaryCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "Employee_GetHRMSummary";

        public override bool IsValid() => EmployeeId > 0;
    }
}