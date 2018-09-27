namespace NexGoal.Criteria.Dashboard
{
    using Fanex.Data.Repository;

    public class GetPARecordCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "Dashboard_GetPARecord";

        public override bool IsValid() => EmployeeId > 0;
    }
}