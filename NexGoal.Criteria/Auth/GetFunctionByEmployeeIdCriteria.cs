namespace NexGoal.Criteria.Auth
{
    using Fanex.Data.Repository;

    public class GetFunctionByEmployeeIdCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override bool IsValid() => EmployeeId > 0;

        public override string GetSettingKey() => "Function_GetByEmployeeId";
    }
}