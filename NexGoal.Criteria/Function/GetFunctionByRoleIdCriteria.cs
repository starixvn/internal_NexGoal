namespace NexGoal.Criteria.Function
{
    using Fanex.Data.Repository;

    public class GetFunctionByRoleIdCriteria : CriteriaBase
    {
        public int RoleId { get; set; }

        public override bool IsValid() => RoleId > 0;

        public override string GetSettingKey() => "Function_GetByRoleId";
    }
}