namespace NexGoal.Criteria.Probation
{
    using Fanex.Data.Repository;

    public class GetHRMCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "Probation_GetHRM";

        public override bool IsValid() => EmployeeId > 0;
    }
}